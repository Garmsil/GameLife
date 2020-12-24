using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Threading;

namespace GameLife
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window, INotifyPropertyChanged //INotifyPropertyChanged нужен для того, чтобы привязать наши свойства с нашей визуальной частью (окном приложения)
    {

        // Клетка = квадраты. Могу писать либо так, либо так
        DispatcherTimer dispatcherTimer = new System.Windows.Threading.DispatcherTimer(); // Класс таймер, который запускает нашу игру и обновляет кадры
        // Есть логическая часть - это наш класс (его еще называют Code-behind), а есть наш интерфейс - это класс MainWindows.xaml.cs
        public MainWindow()
        {
            InitializeComponent();
            DataContext = this; // DataContext нужен для того, чтобы наш интерфейс программы получил все данные для привязки
            canvas.MouseWheel += ReSize; // Событие колесика мыши. Сюда передаем метод, который меняет размер наших квадратиков в игре
            dispatcherTimer.Tick += new EventHandler(Timer1_Tick); // Событие на покадровую отрисовку. Грубо - это старты игры. Вызывает метод каждые уст. секунды
            dispatcherTimer.Interval = new TimeSpan(0, 0, 0, 0, 10); // Интервал обновления - 10 мс. Можно поменять
        }
        Dictionary<Coord, Cell> cells = new Dictionary<Coord, Cell>(); // Словарь, который запоминает наши квадраты по координатам

        private void ReSize(object sender, MouseWheelEventArgs e) // Метод изменения размера квадратов
        {
            canvas.Children.Clear(); // Очищаем полотно
            cells.Clear(); // Очищаем наш словарь
            Cell.ClearItems(); // Очищаем буффер наших клеток
            if (e.Delta > 0) // Если колесико крутим вверх
            {
                Size += 10; // то увеличиваем размер на 10
            }
            else Size -= 10; // иначе уменьшаем, если покрутили вниз
            Draw(); // Отрисовка наших квадратов
        }


        private void Timer1_Tick(object sender, EventArgs e) // Наш таймер
        {
            foreach (var cell in cells) CountCell(cell); // Запускаем метод подсчета соседей для каждой ячейки

            foreach (var cell in cells) // Перечисляем каждую ячейку
            {
                Game(cell.Value); // Передаем ячейку в метод Game
            }

            Count = cells.Values.OfType<Cell>().Where(x => x.Cycle == Cell.CycleLife.New).Count(); // Считаем наши живые клетки
                                                                                                   //Кол-во = словарь.Значения.ТипКлеток.Где(жизненный цикл клетки == живая).СчитаемКоличество
        }

        void Game(Cell cell) // Метод самой логики игры
        {
            if (cell.Cycle == Cell.CycleLife.Empty && cell.CellCount == 3) cell.rect.Fill = cell.GiveLife(); // Если ячейка пустая и рядом 3 соседа, то возрождаем
            if (cell.Cycle == Cell.CycleLife.New && cell.CellCount == 2 || cell.Cycle == Cell.CycleLife.New && cell.CellCount == 3) // Если Ячейка живая и рядом либо 2, либо 3 соседа, то живем
            {
                cell.rect.Fill = cell.GiveLife(); // Красим в красный и даем жизнь клетке
            }
            else // иначе умираем
            {
                cell.rect.Fill = cell.Death(); // Красив в зеленые и омертвляем клетку
            }
        }
