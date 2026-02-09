using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "12cbbdfd6f83dea7a8c3addd9b68059b53bf414c")]
public class AnimInit : Component
{
    [ShowInEditor][ParameterAsset(Filter =".anim")]
    private string EngineAnim;

    [ShowInEditor]
    public ObjectMeshSkinned MainEngine; // Основной объект анимации

    private int totalFrames = 36; // Общее количество кадров в анимации

	private bool play, playReverse = false;
    private bool animationCompleted = false;

    private void Init()
    {
        //MainEngine = (ObjectMeshSkinned)Node.GetNode(359248937); //Айдишник ObjectMeshSkinned (основной объект анимации)
        MainEngine.NumLayers = 1; // Устанавливаем количество слоев анимации в 1

        //int _def = MainEngine.AddAnimation(EngineAnim); // Добавляем анимацию к объекту и получаем идентификатор анимации

        MainEngine.SetLayer(0, true, 1); // Устанавливаем параметры для слоя анимации
        MainEngine.SetLayerAnimationFilePath(0, EngineAnim); // Устанавливаем анимацию для слоя
        
        // Ensure animation starts at frame 0
        MainEngine.SetLayerFrame(0, 0, 0, totalFrames - 1);
    }
    
    private void Update()
    {
		if (Input.IsKeyDown(Input.KEY.E))
		{
			play = true;
			playReverse = false;
            animationCompleted = false;
		}	

		if(Input.IsKeyPressed(Input.KEY.Q))
		{
			play = false;
			playReverse = true;
            animationCompleted = false;
		}			

        if (play && !animationCompleted && MainEngine.GetLayerFrame(0) < totalFrames - 1) // Проверяем нажатие клавиши E и текущий кадр меньше максимального
        {
            float newFrame = MainEngine.GetLayerFrame(0) + Game.IFps * 30;
            if(newFrame >= totalFrames - 1)
            {
                newFrame = totalFrames - 1; // Ограничиваем максимальное значение
                animationCompleted = true; // Mark as completed to maintain final frame
            }
            
            MainEngine.SetLayerFrame(0, newFrame, 0, totalFrames - 1);
			if (MainEngine.GetLayerFrame(0) == totalFrames - 1)
				play = false;
        }

        else if(playReverse && !animationCompleted && MainEngine.GetLayerFrame(0) > 0) // Проверяем нажатие клавиши Q и текущий кадр больше 0
        {
            float newFrame = MainEngine.GetLayerFrame(0) - Game.IFps * 30;
            if(newFrame < 0)
            {
                newFrame = 0; // Ограничиваем минимальное значение
                animationCompleted = true; // Mark as completed to maintain final frame
            }
                
            MainEngine.SetLayerFrame(0, newFrame, 0, totalFrames - 1);
			if (MainEngine.GetLayerFrame(0) == 0)
				playReverse = false;
        }
        
        // When animation is completed, ensure it stays at the final frame
        if (animationCompleted)
        {
            float targetFrame = play ? (totalFrames - 1) : 0;
            MainEngine.SetLayerFrame(0, targetFrame, 0, totalFrames - 1);
        }
    }
}
