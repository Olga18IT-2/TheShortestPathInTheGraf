namespace Graf
{
    class GraphEdge
    {
        public int v1, v2;    // номера вершин, которые соединяет ребро
        public double weight; // вес данного ребра (расстояние между вершинами, которые соединяет ребро)

        public GraphEdge(int v1, int v2, double weight)
        {
            this.v1 = v1;
            this.v2 = v2;
            this.weight = weight;
        }
    }
}