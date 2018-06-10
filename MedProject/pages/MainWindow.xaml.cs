using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace MedProject.pages
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            //Считываем категории, симптомы и болезни из файлов
            Data.GetDataFromFile();

            //Получаем список категорий
            var sections = Data.GetSectionsNames();

            Section.ItemsSource = sections;
            Section.SelectedItem = sections[0];

            DragList.ItemsSource = Data.GetSectionSymptomsNames(sections[0]);
        }

        private ListBox _dragSource;

        //Метод для drag'n'drop
        private void ListBox_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var parent = (ListBox)sender;
            _dragSource = parent;
            var data = GetDataFromListBox(_dragSource, e.GetPosition(parent));

            if (data != null)
            {
                DragDrop.DoDragDrop(parent, data, DragDropEffects.Move);
            }
        }

        //Получение данных из объекта списка
        private static object GetDataFromListBox(ItemsControl source, Point point)
        {
            if (!(source.InputHitTest(point) is UIElement element)) return null;
            var data = DependencyProperty.UnsetValue;
            while (data == DependencyProperty.UnsetValue)
            {
                if (element == null) continue;
                data = source.ItemContainerGenerator.ItemFromContainer(element);

                if (data == DependencyProperty.UnsetValue)
                {
                    element = VisualTreeHelper.GetParent(element) as UIElement;
                }

                if (Equals(element, source))
                {
                    return null;
                }
            }

            return data != DependencyProperty.UnsetValue ? data : null;
        }




        //Метод для drag'n'drop
        private void ListBox_Drop(object sender, DragEventArgs e)
        {

            var data = e.Data.GetData(typeof(string));

            DropList.Items.Add(data);

            var oldData = Data.GetSectionSymptomsNames(Section.SelectedValue.ToString());

            if (DropList.Items.IsEmpty)
            {
                DragList.ItemsSource = oldData;
            }
            else
            {
                var existData = DropList.Items.Cast<string>();
                DragList.ItemsSource = oldData.Where(x => !existData.Contains(x)).ToList();
            }
        }

        //Обработка изменения категории
        private void Section_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            var oldData = Data.GetSectionSymptomsNames(Section.SelectedValue.ToString());

            if (DropList.Items.IsEmpty)
            {
                DragList.ItemsSource = oldData;
            }
            else
            {
                var existData = DropList.Items.Cast<string>();
                DragList.ItemsSource = oldData.Where(x => !existData.Contains(x)).ToList();
            }

        }

        //Обработка нажатия кнопки подсчитать
        private void Btn_OnClick(object sender, RoutedEventArgs e)
        {
            var mySymptoms = DropList.Items.SourceCollection.Cast<string>();

            //Получаем возможную болезнь по симптомам, которые выбрал пользователь
            var prediction = Data.GetDisease(mySymptoms);

            if (MessageBox.Show(
                    $"Посчеты говорят, что вероятнее всего у вас {prediction}.\nХотите получить подробную информацию?",
                    "Подсчёты",
                    MessageBoxButton.YesNo,
                    MessageBoxImage.Asterisk) == MessageBoxResult.Yes)
            {
                System.Diagnostics.Process.Start($"http://simptomer.ru/search?searchword={prediction}");
            }

        }
    }
}
