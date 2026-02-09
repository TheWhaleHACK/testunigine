using System.Collections;
using System.Collections.Generic;
using Unigine;

[Component(PropertyGuid = "cc3fb41e973e59ca2c7127bcbb61982a55f8e584")]
public class AnimControl : Component
{
    [ShowInEditor][ParameterAsset(Filter =".anim")]
    private string EngineAnim;

    [ShowInEditor]
    public ObjectMeshSkinned MainEngine; // Основной объект анимации

    private int totalFrames = 36; // Общее количество кадров в анимации

    private void Init()
    {
        MainEngine = (ObjectMeshSkinned)Node.GetNode(359248937); //Айдишник ObjectMeshSkinned (основной объект анимации)
        MainEngine.NumLayers = 1; // Устанавливаем количество слоев анимации в 1

        int _def = MainEngine.AddAnimation(EngineAnim); // Добавляем анимацию к объекту и получаем идентификатор анимации

        MainEngine.SetLayer(0, true, 1); // Устанавливаем параметры для слоя анимации
        MainEngine.SetAnimation(0, _def); // Устанавливаем анимацию для слоя
    }
    
    private void Update()
    {
        if (Input.IsKeyPressed(Input.KEY.E) && MainEngine.GetFrame(0) < totalFrames - 1) // Проверяем нажатие клавиши E и текущий кадр меньше максимального
        {
            float newFrame = MainEngine.GetFrame(0) + Game.IFps * 30;
            if(newFrame >= totalFrames - 1)
                newFrame = totalFrames - 1; // Ограничиваем максимальное значение
            
            MainEngine.SetFrame(0, newFrame, 0, totalFrames - 1);
        }
        else if(Input.IsKeyPressed(Input.KEY.Q) && MainEngine.GetFrame(0) > 0) // Проверяем нажатие клавиши Q и текущий кадр больше 0
        {
            float newFrame = MainEngine.GetFrame(0) - Game.IFps * 30;
            if(newFrame < 0)
                newFrame = 0; // Ограничиваем минимальное значение
                
            MainEngine.SetFrame(0, newFrame, 0, totalFrames - 1);
        }
    }
}