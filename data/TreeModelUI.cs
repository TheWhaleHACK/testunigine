using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;
using Unigine;

[Component(PropertyGuid = "f16c3e752f604a82f611bfea58efd92c5a298e63")]
public class TreeModelUI : Component
{
	[ShowInEditor]
	private CameraCast cameraCast = null;

	private WidgetScrollBox scrollBox;
	private WidgetButton button;
	private Gui gui;
	private ObjectGui objectGui;
	private TreeOfModel treeOfModel;

	// Переменные для VR
	private InputVRController controllerRight;
	private float scrollSpeed = 500.0f; // Скорость прокрутки
	private float lastTriggerValue = 0.0f; // Для отслеживания изменения триггера

	private bool working = false;
	private int indexObjectCur = 0;

	enum TreeItemState
	{
		ShowOne,
		ChangeCurrent,
		Breadcrumbs
	}


	private class CustomTreeNode
	{
		public WidgetHBox widgetHBox = null;
		public WidgetButton childrenButton = null;
		public WidgetButton showButton = null;
		public WidgetLabel nameLabel = null;

		public string Name { get; set; }
		public int Index { get; set; }
		public List<CustomTreeNode> Children { get; set; }

		public CustomTreeNode(string name, int index)
		{
			Name = name;
			Index = index;
			Children = new List<CustomTreeNode>();
		}
	}

	private CustomTreeNode rootNode = null;
	// private List<CustomTreeNode> treeNodes = new List<CustomTreeNode>();

	void Init()
	{
		treeOfModel = new();
		Node testNode = World.GetNodeByName("1");
		ImportModel.myNode = testNode;
		treeOfModel.CreateTree(testNode);
		scrollSpeed = treeOfModel.treeBox.FontSize * 1500;
		// objectGui = node as ObjectGui;
		// gui = objectGui.GetGui();
		// objectGui.MouseMode = ObjectGui.MOUSE_VIRTUAL;

		controllerRight = Input.VRControllerRight;

		// WidgetVBox vbox = new WidgetVBox(gui, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP | Gui.ALIGN_LEFT);
		// vbox.Background = 1;
		// vbox.BackgroundColor = new vec4(1f, 0f, 0f, 1.0f);

		// scrollBox = new WidgetScrollBox(gui, Gui.ALIGN_EXPAND | Gui.ALIGN_OVERLAP | Gui.ALIGN_LEFT); // Указываем GUI
		// 																							 // scrollBox.Height = gui.Height;
		// 																							 // scrollBox.Width = gui.Width;
		// scrollBox.VScrollEnabled = true;
		// scrollBox.VscrollColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		// scrollBox.Background = 1;
		// scrollBox.BackgroundColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);

		// // scrollBox.AddChild(vbox);
		// gui.AddChild(scrollBox, Gui.ALIGN_EXPAND | Gui.ALIGN_LEFT);

		// indexObjectCur = 0;

		// Node testNode = World.GetNodeByName("1");
		// AssignNodeIndices(testNode, 0, 0);
		// testNode = World.GetNodeByName("1");
		// AddTreeGui(testNode, 0, 0);

		// ImportModel importModel = new ImportModel();
	}

	private void AddTreeGui(Node currentNode, int x, int y)
	{
		if (currentNode == null) return;

		// Создаем новый элемент дерева
		// CustomTreeNode treeNode = new CustomTreeNode(currentNode.Name, indexObjectCur);
		var widgetHBox = new WidgetHBox(gui);
		widgetHBox.SetSpace(5, 0);
		widgetHBox.Background = 1;
		// widgetHBox.BackgroundColor = new vec4(0.5f, 0.5f, 0.5f, 1.0f);

		var childrenButton = new WidgetButton(gui);
		childrenButton.Text = "-";
		childrenButton.FontSize = 16;
		childrenButton.FontColor = new vec4(1.0f, 1.0f, 1.0f, 1.0f);
		if (currentNode.NumChildren != 0)
		{
			widgetHBox.AddChild(childrenButton, Gui.ALIGN_LEFT);
			widgetHBox.SetPadding(x - 38, 0, 0, 0);
		}
		else
		{
			widgetHBox.SetPadding(x, 0, 0, 0);
		}

		var nameLabel = new WidgetLabel(gui);
		nameLabel.Text = currentNode.Name;
		nameLabel.FontSize = 30;
		widgetHBox.AddChild(nameLabel, Gui.ALIGN_LEFT);
		scrollBox.AddChild(widgetHBox, Gui.ALIGN_LEFT);

		// childrenButton.Parent.GetChild(1);

		scrollBox.Arrange();
		// scrollBox.VScrollValue = scrollBox.VScrollObjectSize - scrollBox.VScrollFrameSize;

		for (int i = 0; i < currentNode.NumChildren; i++)
		{
			AddTreeGui(currentNode.GetChild(i), x + 40, y + (i + 1) * 30);
		}
	}

	private void AssignNodeIndices(Node currentNode, int x, int y)
	{
		string strIndex = indexObjectCur.ToString();
		currentNode.SetData("TreeIndex", strIndex);
		currentNode.SetData("ShowNode", "1");

		Log.Message($"Узел: {currentNode.Name}, индекс: {indexObjectCur}\n");

		indexObjectCur++;

		for (int i = 0; i < currentNode.NumChildren; i++)
		{
			AssignNodeIndices(currentNode.GetChild(i), x, y);
		}
	}

	void Update()
	{
		// Управление прокруткой через VR контроллер
		if (controllerRight != null)
		{
			// Используем правый контроллер (можно изменить на левый)
			float triggerValue = controllerRight.GetAxis(1);
			// Log.Message($"Trigger value: {triggerValue}\n");

			if (MathLib.Abs(triggerValue) > 0.1f)
			{
				Object objectGui = cameraCast.GetObject();
				if (objectGui.Name != "TreeGui") return;

				dvec3 controllerDir = controllerRight.GetWorldTransform().GetColumn3(2);
				float scrollDelta = triggerValue * scrollSpeed * Game.IFps;

				// Используем вертикальную компоненту направления контроллера
				float scrollDirection = -(float)controllerDir.y;
				// Log.Message($"Scroll direction: {scrollDirection}\n");

				// Обновляем значение прокрутки
				float newScrollValue = treeOfModel.widgetScrollBox.VScrollValue + scrollDelta;
				// Log.Message($"New scroll value: {newScrollValue}\n");

				// Ограничиваем значение в допустимых пределах
				newScrollValue = MathLib.Clamp(newScrollValue, 0.0f,
					treeOfModel.widgetScrollBox.VScrollObjectSize - treeOfModel.widgetScrollBox.VScrollFrameSize);

				treeOfModel.widgetScrollBox.VScrollValue = (int)newScrollValue;
			}

			// Скрывает все кроме
			if (Input.VRControllerRight.IsButtonDown(Input.VR_BUTTON.Y))
			{
				ChangeItemState(TreeItemState.ShowOne);
			}

			//Скрывает одно
			if (Input.VRControllerRight.IsButtonDown(Input.VR_BUTTON.X))
			{
				ChangeItemState(TreeItemState.ChangeCurrent);
			}
		}
	}

	private void ChangeItemState(TreeItemState itemState)
	{
		Object objectGui = cameraCast.GetObject();
		if (objectGui.Name != "TreeGui") return;

		int index = treeOfModel.treeBox.GetSelectedItem(0);
		if (index == -1) return;

		switch (itemState)
		{
			case TreeItemState.ShowOne:
				treeOfModel.ChangeAllMaterialsState();
				break;
			case TreeItemState.ChangeCurrent:
				treeOfModel.ChangeCurrentMaterialState();
				break;
		}
	}
}