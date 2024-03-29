﻿/**
  Autor: Dalton Solano dos Reis
**/

#define CG_Gizmo
// #define CG_Privado

using System;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System.Collections.Generic;
using OpenTK.Input;
using CG_Biblioteca;
using CG_N2;

namespace gcgcg
{
    class Mundo : GameWindow
    {
        private static Mundo instanciaMundo = null;

        private Mundo(int width, int height) : base(width, height) { }

        public static Mundo GetInstance(int width, int height)
        {
            if (instanciaMundo == null)
                instanciaMundo = new Mundo(width, height);
            return instanciaMundo;
        }

        private SegReta SrPalito;
        private int RaioSrPalito = 100;
        private int AnguloSrPalito = 45;

        private CameraOrtho camera = new CameraOrtho();
        protected List<Objeto> objetosLista = new List<Objeto>();
        private ObjetoGeometria objetoSelecionado = null;
        private char objetoId = '@';
        private bool bBoxDesenhar = false;
        int mouseX, mouseY;   //TODO: achar método MouseDown para não ter variável Global
        private bool mouseMoverPto = false;
        private Retangulo obj_Retangulo;
        private int indexListaTipos = 0;

        private List<PrimitiveType> listaTipos = new List<PrimitiveType>()
        {
            PrimitiveType.Points,
            PrimitiveType.Lines,
            PrimitiveType.LineLoop,
            PrimitiveType.LineStrip,
            PrimitiveType.Triangles,
            PrimitiveType.TriangleStrip,
            PrimitiveType.TriangleFan,
            PrimitiveType.Quads,
            PrimitiveType.QuadStrip,
            PrimitiveType.Polygon
        };
#if CG_Privado
    private Privado_SegReta obj_SegReta;
    private Privado_Circulo obj_Circulo;
#endif

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            camera.xmin = -300; camera.xmax = 300; camera.ymin = -300; camera.ymax = 300;

            Console.WriteLine(" --- Ajuda / Teclas: ");
            Console.WriteLine(" [  H     ] mostra teclas usadas. ");

            //DesenharCirculo(new Ponto4D(0, 0), 100, Color.Yellow, 5);

            //DesenharTrianguloECirculos();

            //DesenharRetanguloMudaFormaPrimitiva();

            DesenharSenhorPalito();



#if CG_Privado
      objetoId = Utilitario.charProximo(objetoId);
      obj_SegReta = new Privado_SegReta(objetoId, null, new Ponto4D(50, 150), new Ponto4D(150, 250));
      obj_SegReta.ObjetoCor.CorR = 255; obj_SegReta.ObjetoCor.CorG = 255; obj_SegReta.ObjetoCor.CorB = 0;
      objetosLista.Add(obj_SegReta);
      objetoSelecionado = obj_SegReta;

      objetoId = Utilitario.charProximo(objetoId);
      obj_Circulo = new Privado_Circulo(objetoId, null, new Ponto4D(100, 300), 50);
      obj_Circulo.ObjetoCor.CorR = 0; obj_Circulo.ObjetoCor.CorG = 255; obj_Circulo.ObjetoCor.CorB = 255;
      objetosLista.Add(obj_Circulo);
      objetoSelecionado = obj_Circulo;
#endif
            GL.ClearColor(0.5f, 0.5f, 0.5f, 1.0f);
        }
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(camera.xmin, camera.xmax, camera.ymin, camera.ymax, camera.zmin, camera.zmax);
        }
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);
            GL.Clear(ClearBufferMask.ColorBufferBit);
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
#if CG_Gizmo
            Sru3D();
#endif
            for (var i = 0; i < objetosLista.Count; i++)
                objetosLista[i].Desenhar();
            if (bBoxDesenhar && (objetoSelecionado != null))
                objetoSelecionado.BBox.Desenhar();
            this.SwapBuffers();
        }
        private void DesenharCirculo(Ponto4D pontoCentral, int raio, Color cor, int tamanho, int pontos = 72, PrimitiveType primitivo = PrimitiveType.Points)
        {
            Circulo circulo = new Circulo(Convert.ToChar("C"), null, pontoCentral, raio, pontos, primitivo);
            circulo.ObjetoCor.CorR = cor.R; circulo.ObjetoCor.CorG = cor.G; circulo.ObjetoCor.CorB = cor.B;
            circulo.PrimitivaTamanho = tamanho;
            objetosLista.Add(circulo);
        }
        private void DesenharTrianguloECirculos()
        {
            Ponto4D ponto1 = new Ponto4D(0, 100, 0);
            Ponto4D ponto2 = new Ponto4D(-100, -100, 0);
            Ponto4D ponto3 = new Ponto4D(100, -100, 0);

            DesenharSegReta(ponto1, ponto2, Color.Cyan);
            DesenharSegReta(ponto2, ponto3, Color.Cyan);
            DesenharSegReta(ponto3, ponto1, Color.Cyan);

            DesenharCirculo(ponto1, 100, Color.Black, 5);
            DesenharCirculo(ponto2, 100, Color.Black, 5);
            DesenharCirculo(ponto3, 100, Color.Black, 5);
        }
        private void DesenharSegReta(Ponto4D ponto1, Ponto4D ponto2, Color cor)
        {
            SegReta segReta1 = new SegReta(Convert.ToChar("C"), null, ponto1, ponto2);
            segReta1.PrimitivaTamanho = 5;
            segReta1.ObjetoCor.CorR = cor.R; segReta1.ObjetoCor.CorG = cor.G; segReta1.ObjetoCor.CorB = cor.B;
            objetosLista.Add(segReta1);
        }
        private void DesenharRetanguloMudaFormaPrimitiva()
        {
            obj_Retangulo = new Retangulo(Convert.ToChar("C"), null, new Ponto4D(200, 200, 0), new Ponto4D(-200, -200, 0));
            obj_Retangulo.DefinirCorPonto(0, Color.MediumPurple);
            obj_Retangulo.DefinirCorPonto(1, Color.Cyan);
            obj_Retangulo.DefinirCorPonto(2, Color.Yellow);
            obj_Retangulo.DefinirCorPonto(3, Color.Black);

            obj_Retangulo.PrimitivaTamanho = 5;
            obj_Retangulo.PrimitivaTipo = listaTipos[0];
            objetosLista.Add(obj_Retangulo);
            objetoSelecionado = obj_Retangulo;
        }
        private void DesenharSenhorPalito()
        {
            Ponto4D pontoCentral = new Ponto4D(0, 0, 0);
            Ponto4D pontoFinal = PontoFinalBaseadoNoAngulo(pontoCentral, AnguloSrPalito, RaioSrPalito);
            SrPalito = new SegReta(Convert.ToChar("C"), null, pontoCentral, pontoFinal);
            SrPalito.ObjetoCor.CorR = 0; SrPalito.ObjetoCor.CorG = 0; SrPalito.ObjetoCor.CorB = 0;
            SrPalito.PrimitivaTamanho = 5;
            objetosLista.Add(SrPalito);
        }
        private Ponto4D PontoFinalBaseadoNoAngulo(Ponto4D ponto, int angulo, int raio)
        {
            Ponto4D pontoFinal = Matematica.GerarPtosCirculo(angulo, raio);
            return new Ponto4D(pontoFinal.X + ponto.X, pontoFinal.Y + ponto.Y, 0);
        }
        protected override void OnKeyDown(OpenTK.Input.KeyboardKeyEventArgs e)
        {
            if (e.Key == Key.H)
                Utilitario.AjudaTeclado();
            else if (e.Key == Key.Escape)
                Exit();
            else if (e.Key == Key.O)
            {
                camera.xmin -= 100;
                camera.xmax += 100;
                camera.ymin -= 100;
                camera.ymax += 100;
            }
            else if (e.Key == Key.I)
            {
                if ((camera.xmin + 100) != 0)
                {
                    camera.xmin += 100;
                    camera.xmax -= 100;
                    camera.ymin += 100;
                    camera.ymax -= 100;
                }
            }
            else if (e.Key == Key.E)
            {
                camera.xmin += 20;
                camera.xmax += 20;
            }
            else if (e.Key == Key.D)
            {
                camera.xmin -= 20;
                camera.xmax -= 20;
            }
            else if (e.Key == Key.C)
            {
                camera.ymin -= 20;
                camera.ymax -= 20;
            }
            else if (e.Key == Key.B)
            {
                camera.ymin += 20;
                camera.ymax += 20;
            }//////
            else if (e.Key == Key.V)
                mouseMoverPto = !mouseMoverPto;
            else if (e.Key == Key.Space)
            {
                if (obj_Retangulo == null)
                    return;

                if (indexListaTipos < listaTipos.Count - 1)
                {
                    indexListaTipos++;
                }
                else
                {
                    indexListaTipos = 0;
                }

                PrimitiveType tipo = listaTipos[indexListaTipos];

                obj_Retangulo.PrimitivaTipo = tipo;
            }
            else if (e.Key == Key.Q)
            {
                if (SrPalito == null)
                    return;

                SrPalito.Ponto1.X--;
                SrPalito.Ponto2.X--;
            }
            else if (e.Key == Key.W)
            {
                if (SrPalito == null)
                    return;

                SrPalito.Ponto1.X++;
                SrPalito.Ponto2.X++;
            }
            else if (e.Key == Key.A)
            {
                if (SrPalito == null)
                    return;

                RaioSrPalito--;
                Ponto4D pontoFinal = PontoFinalBaseadoNoAngulo(SrPalito.Ponto1, AnguloSrPalito, RaioSrPalito);
                SrPalito.Ponto2.X = pontoFinal.X;
                SrPalito.Ponto2.Y = pontoFinal.Y;
            }
            else if (e.Key == Key.S)
            {
                if (SrPalito == null)
                    return;

                RaioSrPalito++;
                Ponto4D pontoFinal = PontoFinalBaseadoNoAngulo(SrPalito.Ponto1, AnguloSrPalito, RaioSrPalito);
                SrPalito.Ponto2.X = pontoFinal.X;
                SrPalito.Ponto2.Y = pontoFinal.Y;
            }
            else if (e.Key == Key.Z)
            {
                if (SrPalito == null)
                    return;

                AnguloSrPalito--;
                Ponto4D pontoFinal = PontoFinalBaseadoNoAngulo(SrPalito.Ponto1, AnguloSrPalito, RaioSrPalito);
                SrPalito.Ponto2.X = pontoFinal.X;
                SrPalito.Ponto2.Y = pontoFinal.Y;
            }
            else if (e.Key == Key.X)
            {
                if (SrPalito == null)
                    return;

                AnguloSrPalito++;
                Ponto4D pontoFinal = PontoFinalBaseadoNoAngulo(SrPalito.Ponto1, AnguloSrPalito, RaioSrPalito);
                SrPalito.Ponto2.X = pontoFinal.X;
                SrPalito.Ponto2.Y = pontoFinal.Y;
            }
            //TODO: falta atualizar a BBox do objeto
            else
                Console.WriteLine(" __ Tecla não implementada.");
        }

        //TODO: não está considerando o NDC
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            mouseX = e.Position.X; mouseY = 600 - e.Position.Y; // Inverti eixo Y
            if (mouseMoverPto && (objetoSelecionado != null))
            {
                objetoSelecionado.PontosUltimo().X = mouseX;
                objetoSelecionado.PontosUltimo().Y = mouseY;
            }
        }

#if CG_Gizmo
        private void Sru3D()
        {
            GL.LineWidth(1);
            GL.Begin(PrimitiveType.Lines);
            // GL.Color3(1.0f,0.0f,0.0f);
            GL.Color3(Convert.ToByte(255), Convert.ToByte(0), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(200, 0, 0);
            // GL.Color3(0.0f,1.0f,0.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(255), Convert.ToByte(0));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 200, 0);
            // GL.Color3(0.0f,0.0f,1.0f);
            GL.Color3(Convert.ToByte(0), Convert.ToByte(0), Convert.ToByte(255));
            GL.Vertex3(0, 0, 0); GL.Vertex3(0, 0, 200);
            GL.End();
        }
#endif
    }
    class Program
    {
        static void Main(string[] args)
        {
            Mundo window = Mundo.GetInstance(600, 600);
            window.Title = "CG_N2";
            window.Run(1.0 / 60.0);
        }
    }
}
