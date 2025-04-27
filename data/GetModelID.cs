using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unigine;
using UnigineApp.data.Code.GUI;

namespace UnigineApp.data.Code
{
    internal class GetModelID
    {

        public async Task<int> GetID()
        {
            GUIGetPathModel getPathModel = new GUIGetPathModel();
            string file = await getPathModel.PathModel();  // Ожидаем выбор файла

            if (string.IsNullOrEmpty(file) || !System.IO.File.Exists(file))
            {
                return 0; // Возвращаем ошибку
            }

            Import_New importer = new Import_New();
            Node result = importer.import(file);

            int ID = result.ID;
   
            //Visualizer.Enabled = true;


            minWorld = new dvec4(0);
            NodeMinZ = 100000.0;

            ProcessNode(result);

            var transform = new mat4(result.Transform);

            var newtransform = new dmat4(transform[0], transform[1], transform[2], transform[3],
                                        transform[4], transform[5], transform[6], transform[7],
                                        transform[8], transform[9], transform[10], transform[11],
                                        transform[12], transform[13], transform[14] - NodeMinZ, transform[15]);

            //var newtransform = new dmat4(transform[0], transform[1], transform[2], transform[3],
            //                            transform[4], transform[5], transform[6], transform[7],
            //                            transform[8], transform[9], transform[10], transform[11],
            //                            0, 0, 0, transform[15]);

            result.WorldTransform = newtransform;

            return (ID);
        }

        dvec4 minWorld = new dvec4(0);
        double NodeMinZ = 100000.0;
        void ProcessNode(Node result)
        {
            if (result == null || result.NumChildren == 0) // Выход из рекурсии
                return;

            for (int i = 0; i < result.NumChildren; i++)
            {
                var Node_1 = result.GetChild(i);
                Node_1.Scale = new vec3(1);

                if (Node_1 is ObjectMeshStatic mesh)
                {
                    mesh.SetIntersection(true, 0);
                    

                    var transform = Node_1.WorldTransform;
                    var scale = transform.Scale; 
                    var bbox = Node_1.BoundBox;

                    dvec3 minLocal = bbox.minimum;

                    minWorld = transform * new vec4(minLocal, 1.0);

                    if (minWorld.z < NodeMinZ)
                    {
                        NodeMinZ = minWorld.z;
                    }

                }

                ProcessNode(Node_1);
            }
        }


    }
}
