using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace MedProject
{
    class Disease
    {

        public Disease(){}

        public Disease(IEnumerable<string> symptoms)
        {
            Symptoms = new ObservableCollection<Symptom>();
            foreach (var symptom in symptoms)
            {
                Symptoms.Add(new Symptom(){Name = symptom});
            }

            Symptoms = new ObservableCollection<Symptom>(Symptoms.OrderBy(x => x.Name));
        }

        public string Name { get; set; }

        public ObservableCollection<Symptom> Symptoms { get; set; }
    }
}
