using System.Collections.ObjectModel;

namespace MedProject
{
    class Section
    {
        public string Name { get; set; }

        public ObservableCollection<Symptom> Symptoms { get; set; }
    }
}
