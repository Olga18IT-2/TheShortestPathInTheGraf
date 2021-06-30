using System;
using System.Drawing;
using System.Windows.Forms;

namespace Graf
{   public partial class Form1 : Form
    {  
        Graph now_graph; // создаём новый граф
        
        // выбранные (выделенные) вершины:
        int select_point_1 = -1; 
        int select_point_2 = -1;

        int[] answer;

        bool move_point = false;

        public Form1()
        {   InitializeComponent();
            toolTip_button1.SetToolTip(button1, "Изменить расположение / переместить существующую вершину(-ы)");
            toolTip_button2.SetToolTip(button2, "Добавить вершину графа");
            toolTip_button3.SetToolTip(button3, "Добавить ребро графа \n(соединить вершины графа)");
            toolTip_button4.SetToolTip(button4, "Удалить элемент(-ы) графа");
            
            Graphics gr = drawing_field.CreateGraphics(); // создаём поле для рисования
            now_graph = new Graph(gr);                    // создаём новый граф

            buttonOnClick(null, null);
        }
        
        // Нажата кнопка (для button1-4)
        private void buttonOnClick(object sender, EventArgs e)
        {   var now_button = (Button)sender;    // текущая кнопка
            foreach (var item in this.Controls) // обходим все элементы
            {   if (item is Button) // проверяем, что это кнопка
                {   
                    // граница кнопки:
                    ((Button)item).FlatStyle = FlatStyle.Flat;
                    ((Button)item).FlatAppearance.BorderSize = 2;
                    Color border_color = new Color();
                    if (item == now_button)
                    //текущую кнопку делаем недоступной для нажатия
                    // (так как нет необходимости в повторном нажатии)
                    // и задаём цвет границы = синий
                    {   now_button.Enabled = false; 
                        border_color = Color.BlueViolet;  }

                    else //остальные кнопки делаем доступными для нажатия
                    // и задём цвет границы = чёрный
                    { ((Button)item).Enabled = true; 
                      border_color = Color.Black;         }
                    ((Button)item).FlatAppearance.BorderColor = border_color;

                }
            }

            string name_button = ""; if (now_button != null) name_button = now_button.Name;
            switch (name_button)
            {   case "button1":
                    { richTextBox1.Text = "Нажмите мышкой на вершину,которую неободимо переместить, " +
                            "\n и, удерживая нажатую кнопку мыши перемещайте вершину до необходимого месторасположения"; break; }
                case "button2":
                    { richTextBox1.Text = "Нажмите мышкой на поле, где желаете расположить новую вершину:"; break; }
                case "button3":
                    {   richTextBox1.Text = "Нажмите правой кнопкой мыши на 2 вершины, которые необходимо соединить ребром." +
                        "\n *** Если была выбрана неправильно вершина(-ы), \n" +
                        "нажмите левой кнопкой на любое место для снятия выделения.";
                        break;
                    }
                case "button4":
                    {   richTextBox1.Text = "Нажмите на вершину / ребро, которые необходимо удалить." +
                        "\n !!! Если была выбрана вершина, которая включается в ребро(-а), \n" +
                        "то соответствующее ребро(-а) также автоматически будут удалены !";
                        break;
                    }
                default: break;
            }
        }



        // нажатие по полю для рисования:
        private void drawing_field_MouseClick(object sender, MouseEventArgs e)
        {   // нажата кнопка "нарисовать новую вершину графа"
            if (button2.Enabled == false)
            {   now_graph.V.Add(new GraphVertex(e.X, e.Y));
                now_graph.G.drawVertex(e.X, e.Y, now_graph.V.Count.ToString(), Color.Black);
                change_Graph_Vertex(now_graph.V.Count);
                richTextBox2.Clear();
            }
            // нажата кнопка "нарисовать ребро графа"
            if(button3.Enabled == false)
            {
                if (e.Button == MouseButtons.Left)
                {   // поиск нужной вершины из списка
                    for (int i = 0; i < now_graph.V.Count; i++)
                    {   // проверкa условия принадлежности точки, в месте щелчка мыши, 
                        // окружности вершины с помощью уравнения окружности: (x – x0)^2 + (y – y0)^2 = R^2.
                        if (Math.Pow((now_graph.V[i].x - e.X), 2) + Math.Pow((now_graph.V[i].y - e.Y), 2) <= now_graph.G.R * now_graph.G.R)
                        {
                            if (select_point_1 == -1) // если первая вершина ещё не была выделена
                            {   // выделяем первую вершину
                                now_graph.G.drawSelectedVertex(now_graph.V[i].x, now_graph.V[i].y); 
                                select_point_1 = i;
                                break;
                            }
                            if (select_point_2 == -1) // если вторая верщина ещё не была выделена
                            {   //выделяем вторую вершину
                                now_graph.G.drawSelectedVertex(now_graph.V[i].x, now_graph.V[i].y);
                                select_point_2 = i;

                                // проверяем, существует ли уже ребро, соединяющее выбранные вершины
                                bool exist_edge = now_graph.ExistEdge(select_point_1, select_point_2);
                                if(exist_edge) { MessageBox.Show("Ребро, соединяющее эти вершины уже существует!"+
                                    "Если желаете добавить ребро именно между данными вершинами, то удалите ранее созданное ребро и повторите попытку!",
                                    "Error"); not_select_point(); return;   }

                                string input_name = "Задаём вес ребра ...";
                                string input_text = "Введите вес ребра " + (select_point_1+1) + " - " + (select_point_2+1) + " :";
                                InputBox.InputBox inputBox = new InputBox.InputBox(input_name, input_text);

                                string input; double weight = 0; // вводимое значение
                                while (true)
                                {
                                    input = inputBox.GetString();
                                    if (inputBox.DialogResult == DialogResult.Cancel) { not_select_point(); return; }
                                    input = input.Replace('.', ','); //заменяет все точки на запятые 
                                    // для отсутствия ошибки при преобразовании в результате некорректного ввода пользователя
                                    try {weight = Convert.ToDouble(input); break; }
                                    catch { }
                                }

                                //добавляем ребро                 
                                now_graph.E.Add(new GraphEdge(select_point_1, select_point_2, weight));
                                now_graph.G.drawEdge(now_graph.V[select_point_1], now_graph.V[select_point_2], 
                                    now_graph.E[now_graph.E.Count - 1], weight, Color.Black);
                                change_Graph_Vertex(now_graph.V.Count);
                                richTextBox2.Clear();

                                select_point_1 = -1;
                                select_point_2 = -1;
                                break;
                            }
                        }
                    }
                }
                if (e.Button == MouseButtons.Right) not_select_point();
            }
            // нажата кнопка "удалить элемент(-ы)"
            if (button4.Enabled == false)
            {   //удалили ли что-нибудь по текущему клику:
                bool temp_flag = false; 

                //проверяем, была ли нажата вершина (то есть вершина для удаления)
                for (int i = 0; i < now_graph.V.Count; i++)
                {   if (Math.Pow((now_graph.V[i].x - e.X), 2) + Math.Pow((now_graph.V[i].y - e.Y), 2) <= now_graph.G.R * now_graph.G.R)
                    { 
                        // проверяем рёбра
                        for (int j = 0; j < now_graph.E.Count; j++)
                        {   // если удаляемая вершина входит в какое-либо ребро
                            if ((now_graph.E[j].v1 == i) || (now_graph.E[j].v2 == i))
                            {   // то удаляем её
                                now_graph.E.RemoveAt(j);
                                j--;
                            }
                            // иначе
                            else
                            {   // если номер вершины, входящей в ребро больше удаляемой вершины,
                                // но уменьшаем на 1 номер вершины (так как 1 вершина была удалена)
                                if (now_graph.E[j].v1 > i) now_graph.E[j].v1--; 
                                if (now_graph.E[j].v2 > i) now_graph.E[j].v2--;
                            }
                        }
                        now_graph.V.RemoveAt(i); // удаляем вершину
                        temp_flag = true; // помечаем, что мы уже что-то удалили
                        break;
                    }
                }
                // если мы ничего ещё не удаляли
                if (temp_flag == false)
                {
                    //проверяем, было ли нажато ребро (то есть ребро для удаления)
                    for (int i = 0; i < now_graph.E.Count; i++)
                    {   if (now_graph.E[i].v1 == now_graph.E[i].v2) //если начальна вершина ребра совпадает с последней
                            // ( то есть это петля )
                        {   if ((Math.Pow((now_graph.V[now_graph.E[i].v1].x - now_graph.G.R - e.X), 2) + 
                                Math.Pow((now_graph.V[now_graph.E[i].v1].y - now_graph.G.R - e.Y), 2) 
                                <= ((now_graph.G.R + 2) * (now_graph.G.R + 2))) 
                                &&
                                (Math.Pow((now_graph.V[now_graph.E[i].v1].x - now_graph.G.R - e.X), 2) + 
                                Math.Pow((now_graph.V[now_graph.E[i].v1].y - now_graph.G.R - e.Y), 2) 
                                >= ((now_graph.G.R - 2) * (now_graph.G.R - 2))))
                            {
                                now_graph.E.RemoveAt(i);   //удаляем ребро (петлю)
                                temp_flag = true;//помечаем, что мы уже что-то удалил
                                break;
                            }
                        }
                        else //если начальная вершина ребра не совпадает с последней
                        // (то есть это полноценное ребро, соединяющее 2 вершины, а не петля)
                        {
                            if (((e.X - now_graph.V[now_graph.E[i].v1].x) * (now_graph.V[now_graph.E[i].v2].y - now_graph.V[now_graph.E[i].v1].y) /
                                (now_graph.V[now_graph.E[i].v2].x - now_graph.V[now_graph.E[i].v1].x) + now_graph.V[now_graph.E[i].v1].y) 
                                <= (e.Y + 4) 
                                &&
                                ((e.X - now_graph.V[now_graph.E[i].v1].x) * (now_graph.V[now_graph.E[i].v2].y - now_graph.V[now_graph.E[i].v1].y) /
                                (now_graph.V[now_graph.E[i].v2].x - now_graph.V[now_graph.E[i].v1].x) + now_graph.V[now_graph.E[i].v1].y) 
                                >= (e.Y - 4))
                            {
                                if ((now_graph.V[now_graph.E[i].v1].x <= now_graph.V[now_graph.E[i].v2].x &&
                                    now_graph.V[now_graph.E[i].v1].x <= e.X && e.X <= now_graph.V[now_graph.E[i].v2].x) 
                                    ||
                                    (now_graph.V[now_graph.E[i].v1].x >= now_graph.V[now_graph.E[i].v2].x &&
                                    now_graph.V[now_graph.E[i].v1].x >= e.X && e.X >= now_graph.V[now_graph.E[i].v2].x))
                                {
                                    now_graph.E.RemoveAt(i);   //удаляем ребро
                                    temp_flag = true;//помечаем, что мы уже что-то удалили
                                    break;
                                }
                            }
                        }
                    }
                }
                //если что-то было удалено, то обновляем граф на экране
                if (temp_flag)
                { now_graph.G.clearField(); now_graph.G.drawALLGraph(now_graph.V, now_graph.E, Color.Black);
                  change_Graph_Vertex(now_graph.V.Count);  richTextBox2.Clear(); }
            }
        }

        //снять выделения вершин:
        private void not_select_point()
        {   if ((select_point_1 != -1))
            {   now_graph.G.drawVertex(now_graph.V[select_point_1].x, now_graph.V[select_point_1].y, 
                (select_point_1 + 1).ToString(), Color.Black);
                select_point_1 = -1;
            }
            if ((select_point_2 != -1))
            {   now_graph.G.drawVertex(now_graph.V[select_point_2].x, now_graph.V[select_point_2].y, 
                (select_point_2 + 1).ToString(), Color.Black);
                select_point_2 = -1;
            }
        }

        // обновляем номера вершин для поиска картчайшего пути:
        private void change_Graph_Vertex (int kolvo)
        {
            comboBox1.Items.Clear(); comboBox2.Items.Clear();
            for(int i=1; i<=kolvo; i++)
            { comboBox1.Items.Add(i); comboBox2.Items.Add(i); }
        }
        private void comboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            richTextBox2.Text = "";
            if (comboBox1.SelectedIndex != -1 && comboBox2.SelectedIndex != -1)
            {

                if (comboBox1.SelectedIndex.ToString() == comboBox2.SelectedIndex.ToString())
                { richTextBox2.Text = "Кратчайший путь = 0 \n (так как исходная вершина графа совпадает с конечной)";
                    answer = new int[1]; answer[0] = comboBox1.SelectedIndex;
                }
                else
                {
                    double result;
                    now_graph.searchPath(comboBox1.SelectedIndex, comboBox2.SelectedIndex, out answer, out result);

                    if (answer.Length != 0)
                    {
                        richTextBox2.Text = "Кратчайший путь из вершины " + (comboBox1.SelectedIndex + 1) + " в вершину " +
                            (comboBox2.SelectedIndex + 1) + " :\n";
                        for (int i = 0; i < answer.Length - 1; i++)
                        {
                            string step = (answer[i] + 1) + "-" + (answer[i + 1] + 1) + "   =   "; // шаг
                            step += now_graph.GetPath(answer[i], answer[i + 1]); // путь (вес) = длине данного шага
                            richTextBox2.Text += step + "\n";
                        }
                        richTextBox2.Text += " ===   Итого: " + result + "   ===";
                    }
                    else
                    {
                        richTextBox2.Text = "Путь из вершины " + (comboBox1.SelectedIndex + 1) +
                          " в вершину " + (comboBox2.SelectedIndex + 1) + " НЕ существует !";
                        answer = new int[0];
                    }
                }
            }
        }

        // нажатие мышкой 
        private void drawing_field_MouseDown(object sender, MouseEventArgs e)
        {
            if(button1.Enabled==false) // если выбрано "перемещение вершины графа"
            {
                for (int i = 0; i < now_graph.V.Count; i++) // определяем выбранную вершину
                {   if (Math.Pow((now_graph.V[i].x - e.X), 2) + Math.Pow((now_graph.V[i].y - e.Y), 2) <= now_graph.G.R * now_graph.G.R)
                    {       now_graph.G.drawSelectedVertex(now_graph.V[i].x, now_graph.V[i].y);
                            select_point_1 = i;
                            move_point = true;
                            break;     
                    }
                }
            }
        }

        // движение зажатой мышкой по полю
        private void drawing_field_MouseMove(object sender, MouseEventArgs e)
        {
            if (button1.Enabled == false) // если выбрано "перемещение вершины графа"
            {
                if (!move_point) return;
                
                // определяем новые координаты точки вершины графа
                // (с учётом невозможности выхода за границы поля drawing_field
                // и того, что радиус вершин = 30)
                int new_x = Math.Min(Math.Max(30, e.X), drawing_field.Width - 30), 
                    new_y = Math.Min(Math.Max(30, e.Y), drawing_field.Height - 30);
                now_graph.V[select_point_1].x = new_x; now_graph.V[select_point_1].y = new_y;
                // перерисовываем граф:
                now_graph.G.clearField(); now_graph.G.drawALLGraph(now_graph.V, now_graph.E, Color.Black);
                now_graph.G.drawSelectedVertex(new_x, new_y);
            }
        }

        // "отпустить" мышку (после нажатия/движения)
        private void drawing_field_MouseUp(object sender, MouseEventArgs e)
        {
            if (button1.Enabled == false) // если выбрано "перемещение вершины графа"
            {   drawing_field_MouseMove(null, e);
                if (select_point_1 != -1) 
                    now_graph.G.drawVertex(now_graph.V[select_point_1].x, now_graph.V[select_point_1].y,
                    (select_point_1+1).ToString(), Color.Black);

                move_point = false; select_point_1 = -1;
            }
        }

        private void richTextBox2_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox2.Text == "") { button5.Visible = false; button6.Visible = false; button6_Click(button6, null); }
            else { button5.Visible = true; button6.Visible = true; }
        }

        private void button6_Click(object sender, EventArgs e) // вернуть исходное форматирование построению
        {
            now_graph.G.clearField(); now_graph.G.drawALLGraph(now_graph.V, now_graph.E, Color.Black);
        }

        private void button5_Click(object sender, EventArgs e) // выделить кратчайший путь
        {
            now_graph.G.clearField(); now_graph.G.drawAllGraph(now_graph.V, now_graph.E, Color.GreenYellow, answer);
        }
    }
}
