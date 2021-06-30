using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Graf
{
    class DrawGraph
    {
        Pen black_pen = new Pen(Color.Black);
        Pen redPen = new Pen(Color.Red);
        Graphics gr;
        Font font_text;
        Brush br;
        PointF point; 
        public int R = 30;

        public DrawGraph(Graphics gr)
        {
            this.gr = gr; clearField();
            black_pen.Width = 2; redPen.Width = 2; 
            font_text = new Font("Arial", 15);    br = Brushes.Black;
        }

        public void clearField()
        {  
            gr.Clear(Color.White);  
        }

        public void drawVertex(int x, int y, string number, Color color)
        {
            Pen my_pen;
            if (color == null)
            {
                my_pen = black_pen;
            }
            else
            {
                my_pen = new Pen(color);
            }

            my_pen.Width = 2;
            gr.FillEllipse(Brushes.White, new Rectangle ((x - R), (y - R), 2 * R, 2 * R ));
            gr.DrawEllipse(my_pen, new Rectangle ((x - R), (y - R), 2 * R, 2 * R));
            point = new PointF(x - 3, y - 3);
            gr.DrawString(number, font_text, br, point);
        }

        public void drawSelectedVertex(int x, int y)
        {  
            gr.DrawEllipse(redPen, (x - R), (y - R), 2 * R, 2 * R); 
        }

        public void drawEdge(GraphVertex V1, GraphVertex V2, GraphEdge E, double weight, Color color)
        {
            if (color == null) color = Color.Black;
            Pen my_pen = new Pen(color);
            
            if (E.v1 == E.v2)
            {   
                gr.DrawArc(my_pen, (V1.x - 2 * R), (V1.y - 2 * R), 2 * R, 2 * R, 90, 270);
                point = new PointF(V1.x - (int)(2.75 * R), V1.y - (int)(2.75 * R));
                gr.DrawString(weight.ToString(), font_text, br, point);
                drawVertex(V1.x, V1.y, (E.v1 + 1).ToString(), color);
            }
            else
            {   
                gr.DrawLine(my_pen, V1.x, V1.y, V2.x, V2.y);
                point = new PointF((V1.x + V2.x) / 2, (V1.y + V2.y) / 2);
                gr.DrawString(weight.ToString(), font_text, br, point);
                drawVertex(V1.x, V1.y, (E.v1 + 1).ToString(), color);
                drawVertex(V2.x, V2.y, (E.v2 + 1).ToString(), color);
            }
        }

        public void drawALLGraph(List<GraphVertex> V, List<GraphEdge> E, Color color)
        {
            for (int i = 0; i < E.Count; i++)  
                drawEdge(V[E[i].v1], V[E[i].v2], E[i], E[i].weight, color);

            for (int i = 0; i < V.Count; i++) 
                drawVertex(V[i].x, V[i].y, (i + 1).ToString(), color);
        }

        public void drawAllGraph(List<GraphVertex>V, List<GraphEdge> E, Color color, int[] path)
        {
            for (int i = 0; i < E.Count; i++)
            {
                int nomer_1 = -1, nomer_2=-1;
                for(int j=0; j<path.Length; j++)
                { 
                    if (path[j] == E[i].v1) 
                    {
                        nomer_1 = j; break; 
                    } 
                }

                if (nomer_1 != -1) 
                { 
                    if(nomer_1+1<path.Length) 
                        if (path[nomer_1 + 1] == E[i].v2) 
                            nomer_2 = nomer_1+1; 

                    if(nomer_1-1>=0) 
                        if (path[nomer_1 - 1] == E[i].v2) 
                            nomer_2 = nomer_1-1;
                }

                Color draw_edge = Color.Black;
                if (nomer_1 != -1 && nomer_2 != -1) 
                    draw_edge = color;
                drawEdge(V[E[i].v1], V[E[i].v2], E[i], E[i].weight, draw_edge); 
            }

            for (int i = 0; i < V.Count; i++)
            {
                Color draw_vertex = Color.Black;
                for (int j=0; j<path.Length; j++)
                { 
                    if (i == path[j]) 
                    {
                        draw_vertex = color;
                        break;
                    } 
                }
                drawVertex(V[i].x, V[i].y, (i + 1).ToString(), draw_vertex); 
            }
        }

    }
}