using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;

namespace UnigineApp.data.Code.GUI
{
    internal class GUIGetPathModel
    {
        EventConnections connections = new EventConnections();
        GetModelID getmodelID = new GetModelID();
        public async Task<string> PathModel()
        {
            var tcs = new TaskCompletionSource<string>();

            // Создаем диалоговое окно
            WidgetDialogFile dialogFile = new WidgetDialogFile(Gui.GetCurrent(), "Файлы");
            dialogFile.Path = "./";
            dialogFile.Filter = ".fbx.stl.stp.step.obj";

            dialogFile.GetOkButton().EventClicked.Connect(connections, () =>
            {
                // Получение пути к файлу
                string path = dialogFile.File;

                dialogFile.Hidden = true;
                dialogFile.RemoveFocus();

                tcs.SetResult(path); // Возвращаем путь
            });

            dialogFile.GetCancelButton().EventClicked.Connect(connections, () =>
            {
                dialogFile.Hidden = true;
                dialogFile.RemoveFocus();

                tcs.SetResult(null); // null если пользователь отменил
            });

            // Добавляем диалог в текущий GUI
            Gui.GetCurrent().AddChild(dialogFile, Gui.ALIGN_OVERLAP | Gui.ALIGN_CENTER);

            // Устанавливаем постоянный фокус на диалог
            dialogFile.SetPermanentFocus();

            return await tcs.Task;
        }

    }
}
