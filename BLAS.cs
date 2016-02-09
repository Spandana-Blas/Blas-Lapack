using System;
using Microsoft.AspNet.SignalR;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Demo_Lapack
{
    public class BLAS : Hub
    {
        public double[,] P;
        public double[,] result = new double[2, 1];
        public double Blas1(String mat1, String mat2)
        {
            //Deserialize (JSON --> C# Object) using Newtonsoft.Json namespace  http://www.newtonsoft.com/json
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);  // see below for MatObject class
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);  // see below for MatObject class
            
            //dot product: C = C + A*B
            int N = A.size[1]; // the number of elements in vector A 
            double C = 0; // dot product C
            for (int i = 0; i < N; i++)
            {
                C = C + A.data[0, i] * B.data[i, 0];
            }
            String strC = C.ToString();

            // Call the displayBlas1  method on the client side.
            Clients.All.DisplayBlas1(strC);

            return C;
        }

        public Matrix Blas2(String mat1, String mat2)
        {
            //Deserialize (JSON --> C# Object) using Newtonsoft.Json namespace  http://www.newtonsoft.com/json
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);  
            var B = JsonConvert.DeserializeObject<Matrix>(mat2); 
            
            int N = A.size[1]; 
            double C = 0; // dot product C
            Matrix G = new Matrix(); // result of the matrix - vector multiplication stored in G
            G.data = new double[3, 3];
            String toPass = null;
            for (int i=0; i<N; i++)
            {
                for(int j=0; j<B.size[1];j++)
                {
                    G.data[i, j] = 0;
                }
            }
            for (int i = 0; i< N; i++)
            {
                C = 0;
                for (int k = 0; k < N; k++)
                {
                    C = C + A.data[i, k] * B.data[k, 0];
                    
                }
                G.data[i, 0] = C;
                String strG = G.data[i, 0].ToString();
                toPass += '['+strG+']';
                if (i != (N-1))
                {
                    toPass += ',';
                }
                
            }
            String strH = "{\"data\": ["+toPass+"]}";
            Clients.All.DisplayBlas2(strH);
            return G;
        }
        public Matrix Blas3(String mat1, String mat2)
        {
            //Deserialize (JSON --> C# Object) using Newtonsoft.Json namespace  http://www.newtonsoft.com/json
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);

            int N = A.size[1];
            double C = 0; // dot product C
            Matrix G = new Matrix(); // result of the matrix - vector multiplication stored in G
            G.data = new double[10, 10];
            String toPass1 = null;
            String toPass = null;
            int P = A.size[0];
            int Q = A.size[1];
            int R = B.size[0];
            int S = B.size[1];
            if (Q == R)
            {
                for (int i = 0; i < P; i++)
                {
                    for (int j = 0; j < S; j++)
                    {
                        G.data[i, j] = 0;
                    }
                }

                for (int i = 0; i < P; i++)
                {
                    toPass = "";
                    for (int j = 0; j < S; j++)
                    {
                        C = 0;
                        for (int k = 0; k < R; k++)
                        {
                            C = C + (A.data[i, k] * B.data[k, j]);
                        }
                        G.data[i, j] = C;
                        String strG = G.data[i, j].ToString();
                        //toPass is the string with row vectors
                        toPass += strG;
                        if (j != (S-1))
                        {
                            toPass += ',';
                        }
                    }

                    
                    toPass1 += '[' + toPass + ']';
                    if (i != (Q - 1))
                    {
                        toPass1 += ',';
                    }

                }
                String strH = "{\"data\": [" + toPass1 + "]}";
                Clients.All.DisplayBlas3(strH);
            }
            return G;
        }
        public class Matrix
        {
            public string matrixType { get; set; }
            public double[,] data { get; set; }
            public int[] size { get; set; }
        }
    }
}
