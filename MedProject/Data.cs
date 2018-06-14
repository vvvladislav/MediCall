using System;
using System.IO;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Newtonsoft.Json;

namespace MedProject
{
    class Data
    {
        private static ObservableCollection<Disease> _diseases;
        private static ObservableCollection<Section> _sections;

        /// <summary>
        /// Метод считывания данных из файлов
        /// </summary>
        public static void GetDataFromFile()
        {


            var rowSections = File.ReadAllText("Sections.json");
            _sections = new ObservableCollection<Section>(JsonConvert.DeserializeObject<ObservableCollection<Section>>(rowSections).OrderBy(x => x.Name));

            foreach (var item in _sections)
            {
                item.Symptoms = new ObservableCollection<Symptom>(item.Symptoms.OrderBy(x => x.Name));
            }


            var rowDiseases = File.ReadAllText("Diseases.json");
            _diseases = JsonConvert.DeserializeObject<ObservableCollection<Disease>>(rowDiseases);

            foreach (var item in _diseases)
            {
                item.Symptoms = new ObservableCollection<Symptom>(item.Symptoms.OrderBy(x => x.Name));
            }

        }
        
        /// <summary>
        /// Метод, который находит к какой категории относится данный симптом
        /// </summary>
        /// <param name="symptom"></param>
        /// <returns></returns>
        public static string ComeBack(string symptom)
        {
            return _sections.Single(x => x.Symptoms.Select(y=>y.Name).Contains(symptom)).Name;
        }

        /// <summary>
        /// Метод получения списка категорий
        /// </summary>
        /// <returns></returns>
        public static ObservableCollection<string> GetSectionsNames()
        {
            return new ObservableCollection<string>(
                _sections.ToList()
                .ConvertAll(input => input.Name)
                );
        }

        /// <summary>
        /// Метод получения списка симптомов для конкретной категории
        /// </summary>
        /// <param name="name">Название категории</param>
        /// <returns></returns>
        public static ObservableCollection<string> GetSectionSymptomsNames(string name)
        {
            if (name == "") return null;
            return new ObservableCollection<string>(
                _sections.SingleOrDefault(input => input.Name == name)
                .Symptoms
                .ToList()
                .ConvertAll(input => input.Name)
                );
        }

        /// <summary>
        /// Метод поиска наиболее подходящей под симптомы пользователя болезни
        /// 
        /// Для поиска подходящей болезни представим, что список симптомов пользователя - болезнь
        /// Тогда, если посчитать расстояния Левенштейна от псевдо-болезни пользователя до каждой существующей болезни, 
        /// то существующая болезнь с минимальным расстоянием и будет наиболее подходящей под описание пользователя
        /// </summary>
        /// <param name="symptoms">Список симптомов пользователя</param>
        /// <returns></returns>
        public static string GetDisease(IEnumerable<string> symptoms)
        {

            var myDisease = new Disease(symptoms);


            var minLen = 100000;
            var predictName = "";


            foreach (var disease in _diseases)
            {
                var curLen = CountLevenshtainLen(myDisease, disease);
                if (curLen >= minLen) continue;
                minLen = curLen;
                predictName = disease.Name;
            }

            return predictName;
        }

        /// <summary>
        /// Поиск расстояния Левенштейна
        /// </summary>
        /// <param name="source">Болезнь пользователя</param>
        /// <param name="target">Существущая болезнь</param>
        /// <returns></returns>
        private static int CountLevenshtainLen(Disease source, Disease target)
        {
            if (source == null)
                return target?.Symptoms.Count ?? 0;


            if (target == null) return source.Symptoms.Count;

            var m = target.Symptoms.Count;
            var n = source.Symptoms.Count;
            var distance = new int[2, m + 1];

            for (var j = 1; j <= m; j++) distance[0, j] = j;

            var currentRow = 0;

            for (var i = 1; i <= n; ++i)
            {
                currentRow = i & 1;
                distance[currentRow, 0] = i;
                var previousRow = currentRow ^ 1;
                for (var j = 1; j <= m; j++)
                {
                    var cost = (target.Symptoms[j - 1].Name == source.Symptoms[i - 1].Name ? 0 : 1);
                    distance[currentRow, j] =
                        Math.Min(
                                Math.Min(
                                        distance[previousRow, j] + 1,
                                        distance[currentRow, j - 1] + 1
                                        ),
                                distance[previousRow, j - 1] + cost
                                );
                }
            }

            return distance[currentRow, m];
        }

        /// <summary> 
        /// Метод, который находит к какой категории относится данный симптом 
        /// </summary> 
        /// <param name="symptom"></param> 
        /// <returns></returns> 
        public static string ComeBack(string symptom)
        {
            return _sections.SingleOrDefault(x => x.Symptoms.Select(y => y.Name).Contains(symptom)).Name;
        }
    }
}
