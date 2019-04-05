
namespace CsLaboratory5.Models
{
    internal class CustomModule
    {
        public string ModuleName { get; set; }
        public string ModulePath { get; set; }

        public CustomModule(string name, string path)
        {
            ModuleName = name;
            ModulePath = path;
        }
    }
}
