using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyGame.Scenes
{
    public class Entity
    {
        public string name { get; set; }
        public int id { get; set; }
        public string _eid { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public int originX { get; set; }
        public int originY { get; set; }
    }

    public class Layer
    {
        public string name { get; set; }
        public string _eid { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public int gridCellWidth { get; set; }
        public int gridCellHeight { get; set; }
        public int gridCellsX { get; set; }
        public int gridCellsY { get; set; }
        public string tileset { get; set; }
        public List<List<int>> data2D { get; set; }
        public int exportMode { get; set; }
        public int arrayMode { get; set; }
        public List<Entity> entities { get; set; }
    }

    public class LevelJson
    {
        public string ogmoVersion { get; set; }
        public int width { get; set; }
        public int height { get; set; }
        public int offsetX { get; set; }
        public int offsetY { get; set; }
        public List<Layer> layers { get; set; }
    }


}
