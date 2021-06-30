using System.Collections.Generic;
using System.Drawing;

namespace Graf
{   
    class Graph
    {   
        public List<GraphVertex> V; // список вершин графа
        public List<GraphEdge> E;   // список рёбер графа
        public double[,] Matrix;    //матрица смежности
        public DrawGraph G;         // поле для рисования

        public Graph(Graphics gr)
        {   
            V = new List<GraphVertex>();
            E = new List<GraphEdge>();
            G = new DrawGraph(gr);
        }

        public bool ExistEdge(int v1, int v2)
        {  
            bool answer = false;
            for (int i = 0; i < E.Count; i++)
            {   
                if ((v1 == E[i].v1 && v2 == E[i].v2) ||
                   (v2 == E[i].v1 && v1 == E[i].v2))
                { 
                    answer = true;
                    break; 
                }
            }

            return answer;
        }

        public double GetPath(int v1, int v2)
        {   
            double weight = -1;
            if (ExistEdge(v1, v2))
            {
                for (int i = 0; i < E.Count; i++)
                {
                    if ((v1 == E[i].v1 && v2 == E[i].v2) ||
                       (v2 == E[i].v1 && v1 == E[i].v2))
                    { 
                        weight = E[i].weight; 
                    }
                }
            }

            return weight;
        }
        
        public void GetMatrix()
        {   
            int n = V.Count;
            Matrix = new double[n, n];
            for (int i=0; i<n; i++)
                for(int j=0; j<n; j++)
                {   
                    if (i == j) 
                        Matrix[i, j] = 0;
                    else 
                        Matrix[i, j] = double.PositiveInfinity;
                }

            for (int i = 0; i < n; i++)
                for (int j = 0; j < n; j++)
                {   
                    double path = GetPath(i, j);
                    if (path!=-1) 
                        Matrix[i, j] = path;
                }
        }

        public void searchPath(int a, int b, out int[]answer, out double result)
        {
            int n = V.Count;
            int source = a;   
            double[] dist = new double[n];  
            int[] from = new int[n]; 
            
            GetMatrix();
            
            for (int i = 0; i < n; i++) 
                dist[i] = double.PositiveInfinity;

            dist[source] = 0;
            bool[] processed = new bool[n]; 
            
            for (int i = 0; i < n; i++)
            {   
                if (!double.IsPositiveInfinity(Matrix[source, i]) && i != source)
                {   
                    dist[i] = Matrix[source, i];
                    from[i] = source;
                }
            }
            processed[source] = true;

            while (continueProcess(processed))
            {
                int checkNode = -1;

                for (int i = 0; i < n; i++)
                {    
                    if (!processed[i] && (checkNode == -1 || dist[checkNode] > dist[i]))
                        checkNode = i;
                }

                for (int i = 0; i < n; i++)
                {   
                    if (!double.IsPositiveInfinity(Matrix[checkNode, i]) && i != checkNode &&
                            dist[i] > (dist[checkNode] + Matrix[checkNode, i]))
                    {   
                        dist[i] = dist[checkNode] + Matrix[checkNode, i];
                        from[i] = checkNode;
                    }
                }
                
                processed[checkNode] = true;
            }

            result = dist[b];
            if (result != double.PositiveInfinity) 
            {
                int[] path = new int[n];
                int v_now = b, kolvo_path = 1; path[0] = v_now;
                do
                {
                    v_now = from[path[kolvo_path - 1]];
                    path[kolvo_path] = v_now;
                    kolvo_path++;
                } 
                while (v_now != source);

                answer = new int[kolvo_path]; 
                int i = 0;
                for (int j = kolvo_path - 1; j >= 0; j--)
                {
                    answer[i] = path[j];
                    i++;
                }
            }
            else 
            { 
                answer = new int[0];  
            }
        }

        private bool continueProcess(bool[] processed)
        {
            foreach (bool el in processed)
                if (!el)
                {
                    return true;
                }
            return false;
        }
    }
}