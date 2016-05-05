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
        public Matrix Blas4(String mat1, String mat2)
        {
            Matrix L = new Matrix();
            Matrix U = new Matrix();
            Matrix Y = new Matrix();
            Matrix X = new Matrix();
            String toPass = null;
            String toPass1 = null;

            //String toPass1 = null;
            L.data = new double[10, 10];
            U.data = new double[10, 10];
            Y.data = new double[10, 10];
            X.data = new double[10, 10];
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);
            int P = A.size[0];
            int Q = A.size[1];
            int R = B.size[0];
            
            int S = B.size[1];
            for (int a = 0; a < P; a++)
            {

                for (int b = 0; b < P; b++)
                {
                    L.data[a, b] = 0;
                    U.data[a, b] = 0;
                    Y.data[a, b] = 0;
                    X.data[a, b] = 0;
                }
               
            }

            for (int a = 0; a < P; a++)
            {
                for (int b = 0; b < P; b++)
                {
                    if (a <= b)
                    {
                        U.data[a, b] = A.data[a, b];
                        for (int c = 0; c <= a - 1; c++)
                        {
                            U.data[a, b] -= (L.data[a, c] * U.data[c, b]);
                            U.data[a, b] = Math.Round(U.data[a, b], 2);
                        }
                        if (a == b)
                            L.data[a, b] = 1;
                        else
                            L.data[a, b] = 0;

                    }
                    else
                    {
                        L.data[a, b] = A.data[a, b];
                        for (int c = 0; c <= b - 1; c++)
                        {
                            L.data[a, b] -= (L.data[a, c] * U.data[c, b]);
                            L.data[a, b] = Math.Round(L.data[a, b], 2);
                        }

                        L.data[a, b] /= U.data[b, b];
                        L.data[a, b] = Math.Round(L.data[a, b], 2);
                        U.data[a, b] = 0;
                    }
                }
            }
            //Now we solve Y where LY = B
            for (int a = 0; a < P; a++)
            {
                Y.data[a, 0] = B.data[a, 0];
                for (int b = 0; b < a; b++)
                {
                    Y.data[a, 0] = Y.data[a, 0] - (L.data[a, b] * Y.data[b, 0]);
                    Y.data[a, 0] = Math.Round(Y.data[a, 0], 2);
                }
            }
            for (int a = P - 1; a >= 0; a--)
            {
                int b;
                X.data[a, 0] = Y.data[a, 0];
                for (b = a + 1; b < P; b++)
                {
                    X.data[a, 0] -= (U.data[a, b] * X.data[b, 0]);
                    X.data[a, 0] = Math.Round(X.data[a, 0], 2);
                }
                X.data[a, 0] /= U.data[a, a];
                X.data[a, 0] = Math.Round(X.data[a, 0], 2);
                
                //toPass += strX;
            }

            int h = 0;

            for (int a = 0; a < Q; a++)
            {
                h = h + 1;
                                String strX = X.data[a, 0].ToString();
                if (h != Q)
                {
                    toPass += '[' + strX + ']' + ',';
                }
                else
                {
                    toPass += '[' + strX + ']';
                }
            

            }

            //toPass1 = "[" + toPass +"]";
            //String strH = "{\"data\": ["+ toPass+"]}";
            String strH = "{\"data\": ["+toPass+"]}";
            Clients.All.DisplayBlas4(strH);

            return L;


        }

        public Matrix Blas5(String mat1, String mat2)
        {
            Matrix U = new Matrix();
            Matrix Q = new Matrix();
            Matrix R = new Matrix();
            Matrix Qt = new Matrix();
            Matrix QtB = new Matrix();
            Matrix X = new Matrix();
            Matrix Lower = new Matrix();
            Matrix Upper = new Matrix();
            Matrix X2 = new Matrix();
            Matrix Y2 = new Matrix();

            String toPass1 = null;

            //String toPass1 = null;
            
            var A = JsonConvert.DeserializeObject<Matrix>(mat1);
            var B = JsonConvert.DeserializeObject<Matrix>(mat2);
            int m = A.size[0];
            int n = A.size[1];

            
            U.data = new double[m, n];
            Q.data = new double[m, n];
            Qt.data = new double[n, m];
            R.data = new double[n, n];
            QtB.data = new double[n, 1];
            X.data = new double[n, 1];
            Lower.data = new double[10, 10];
            Upper.data = new double[10, 10];
            X2.data = new double[10, 10];
            Y2.data = new double[10, 10];
            String toPass = null;
            //String toPassY = null;
            
            double norm;
            double[] abc;
            for (int j = 0; j < n; j++)
            {
                Matrix projection = new Matrix();
                projection.data = new double[j, 1];
                abc = new double[j];

                for (int i = 0; i < m; i++)
                {
                    U.data[i, j] = A.data[i, j];
                    for (int p = 0; p < j; p++)
                    {
                        abc[p] += A.data[i, j] * Q.data[i, p];
                    }

                }
                norm = 0;
                for (int i = 0; i < m; i++)
                {
                    for (int p = 0; p < j; p++)
                    {
                        U.data[i, j] = U.data[i, j] - (abc[p] * Q.data[i, p]);
                    }
                    norm += Math.Pow(U.data[i, j], 2);
                }
                norm = Math.Sqrt(norm);
                for (int i = 0; i < m; i++)
                {
                    Q.data[i, j] = U.data[i, j] / norm;
                }

            }

            for (int i = 0; i < n; i++)
            {
                for (int j = i; j < n; j++)
                {
                    for (int p = 0; p < m; p++)
                    {
                        R.data[i, j] += A.data[p, j] * Q.data[p, i];

                    }
                }
            }


                
             
            // Rx = Transpose(Q) * b should be solved now 

            // First find transpose of Q
            for (int i = 0; i < m; i++)
            {
                for (int j = 0; j < n; j++)
                {
                    
                    Qt.data[j, i] = Q.data[ i, j];
                }
            }

            // Now multiply Transpose of Q i.e. Qt and b
            for (int i = 0; i < n; i++)
            {
                for (int j = 0; j < B.size[1]; j++)
                {
                    double C = 0;
                    for (int k = 0; k < B.size[0]; k++)
                    {
                        C = C + (Qt.data[i, k] * B.data[k, j]);
                    }
                    QtB.data[i, j] = C;
                    //toPass is the string with row vectors

                }
            }

            // Now solve the System R X = QtB

            int u = n;
            int v = n;
            int w = n;

            int o = B.size[1];


            for (int a = 0; a < u; a++)
            {
                for (int b = 0; b < u; b++)
                {
                    if (a <= b)
                    {
                        Upper.data[a, b] = R.data[a, b];
                        for (int c = 0; c <= a - 1; c++)
                        {
                            Upper.data[a, b] -= (Lower.data[a, c] * Upper.data[c, b]);
                            Upper.data[a, b] = Math.Round(Upper.data[a, b], 2);
                        }
                        if (a == b)
                            Lower.data[a, b] = 1;
                        else
                            Lower.data[a, b] = 0;

                    }
                    else
                    {
                        Lower.data[a, b] = R.data[a, b];
                        for (int c = 0; c <= b - 1; c++)
                        {
                            Lower.data[a, b] -= (Lower.data[a, c] * Upper.data[c, b]);
                            Lower.data[a, b] = Math.Round(Lower.data[a, b], 2);
                        }

                        Lower.data[a, b] /= Upper.data[b, b];
                        Lower.data[a, b] = Math.Round(Lower.data[a, b], 2);
                        Upper.data[a, b] = 0;
                    }
                }
            }
            //Now we solve Y where LY = B
            for (int a = 0; a < u; a++)
            {
                Y2.data[a, 0] = QtB.data[a, 0];
                for (int b = 0; b < a; b++)
                {
                    Y2.data[a, 0] = Y2.data[a, 0] - (Lower.data[a, b] * Y2.data[b, 0]);
                    Y2.data[a, 0] = Math.Round(Y2.data[a, 0], 2);
                }
            }
            for (int a = u - 1; a >= 0; a--)
            {
                int b;
                X2.data[a, 0] = Y2.data[a, 0];
                for (b = a + 1; b < u; b++)
                {
                    X2.data[a, 0] -= (Upper.data[a, b] * X2.data[b, 0]);
                    X2.data[a, 0] = Math.Round(X2.data[a, 0], 2);
                }
                X2.data[a, 0] /= Upper.data[a, a];
                X2.data[a, 0] = Math.Round(X2.data[a, 0], 2);
                String strX = X2.data[a, 0].ToString();
                //toPass += strX;int h = 0;
            }
            int h = 0;
            for (int a = 0; a < n; a++)
            {
                h = h + 1;
                String strX = X2.data[a, 0].ToString();
                if (h != n)
                {
                    toPass += '[' + strX + ']' + ',';
                }
                else
                {
                    toPass += '[' + strX + ']';
                }


            }

                //toPass1 = "[" + toPass +"]";
                String strH = "{\"data\": [" + toPass + "]}";
                Clients.All.DisplayBlas5(strH);

            return Q;


        }

        public Matrix Blas6(String mat1)
        {

            String toPass1 = null;

            //String toPass1 = null;

            var A = JsonConvert.DeserializeObject<Matrix>(mat1);
            //var B = JsonConvert.DeserializeObject<Matrix>(mat2);
            int m = A.size[0];
            int n = A.size[1];


            String toPass = null;
            //String toPassY = null;
            String strH = null;


            double norm;
            double C = 0;
            double[] abc;

            for (int t = 0; t < 100; t++)
            {
                double[,] R1 = new double[m, n];
                double[,] Q1 = new double[m, n];
                Matrix U = new Matrix();
                U.data = new double[m, n];
                strH = "";

                for (int j = 0; j < n; j++)
                {
                    Matrix projection = new Matrix();
                    projection.data = new double[j, 1];
                    abc = new double[j];

                    for (int i = 0; i < m; i++)
                    {
                        U.data[i, j] = A.data[i, j];
                        for (int p = 0; p < j; p++)
                        {
                            abc[p] += A.data[i, j] * Q1[i, p];
                        }

                    }
                    norm = 0;
                    for (int i = 0; i < m; i++)
                    {
                        for (int p = 0; p < j; p++)
                        {
                            U.data[i, j] = U.data[i, j] - (abc[p] * Q1[i, p]);
                        }
                        norm += Math.Pow(U.data[i, j], 2);
                    }
                    norm = Math.Sqrt(norm);
                    for (int i = 0; i < m; i++)
                    {
                        Q1[i, j] = U.data[i, j] / norm;
                        //Q1[i, j] = Math.Round(Q1[i, j], 2);
                    }

                }

                for (int i = 0; i < n; i++)
                {
                    for (int j = i; j < n; j++)
                    {
                        for (int p = 0; p < m; p++)
                        {
                            R1[i, j] += A.data[p, j] * Q1[p, i];
                            //R1[i, j] = Math.Round(R1[i, j], 2);

                        }
                    }
                }

                int P = R1.GetLength(0);
                int Q = R1.GetLength(1);
                int R = Q1.GetLength(0);
                int S = Q1.GetLength(1);
                toPass1 = "";

                //Recompute A = R*Q
                for (int i = 0; i < P; i++)
                {
                    toPass = "";
                    for (int j = 0; j < S; j++)
                    {
                        C = 0;
                        for (int k = 0; k < R; k++)
                        {
                            C = C + (R1[i, k] * Q1[k, j]);
                            //C = Math.Round(C, 2);
                        }

                        A.data[i, j] = C;
                        if (i == j)
                        {
                            String strA = A.data[i, j].ToString();
                            toPass += strA;

                        }
                        
                        //toPass is the string with row vectors
                        
                        
                        
                    }
                    
                    toPass1 += '[' + toPass + ']';
                    if (i != (Q - 1))
                    {
                        toPass1 += ',';
                    }
                

                }
                strH = "{\"data\": [" + toPass1 + "]}";
            }
            
            Clients.All.DisplayBlas6(strH);

            return A;
        }
        public class Matrix
        {
            public string matrixType { get; set; }
            public double[,] data { get; set; }
            public int[] size { get; set; }
        }
    }
}
