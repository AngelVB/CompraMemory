using System;
using System.Collections.Generic;
using CocosSharp;

namespace GoneBananas
{

	//Pantalla inicial.
    public class GameStartLayer : CCLayerColor
    {
		public static List<ClaseProducto> listaProductos;

		public static List<ClaseProducto> listaCompra;
	
		public static int NumProductos,traidos;

		public static CCPoint Escala;

		CCSprite fondo;
        //Constructor
		public GameStartLayer (int s) : base ()
        {
			Escala = GoneBananasApplicationDelegate.scale;

			//Listener para controlar la pulsación en pantalla. 
            var touchListener = new CCEventListenerTouchAllAtOnce ();

			//Al pulsar, cargamos la escena principal de juego.
            touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameLayer.GameScene (Window));

			//Añadimos el evento
            AddEventListener (touchListener, this);

			//Configuramos el color de fondo de la escena y su opacidad
            Color = CCColor3B.Black;
            Opacity = 255;

			AddFondo ();

			listaProductos = new List<ClaseProducto> ();


			var spriteSheet = new CCSpriteSheet ("products.plist");


			//Almacenamos todos los productos en un array.
			foreach (var pr in spriteSheet.Frames) {

				var prod = new ClaseProducto ();
				prod.nombre = pr.TextureFilename.Substring(0,pr.TextureFilename.Length-4);
				prod.imagen = new CCSprite (pr.Texture,pr.TextureRectInPixels);
				prod.TamanyoyPos= pr.TextureRectInPixels;
				prod.Cogido = false;
				listaProductos.Add (prod);
			}
			NumProductos = s;
			listaCompra = new List<ClaseProducto> ();


			//Generamos lista de la compra aleatoria con el número de productos dependiendo del nivel.
			int idx;
			for (int i = 0; i < NumProductos; i++) {

				do{
					idx = (CCRandom.GetRandomInt (0,listaProductos.Count-1));
				}while (listaCompra.Contains(listaProductos[idx]));
				listaCompra.Add(listaProductos[idx]);

			}

        }

		void AddFondo ()
		{
			fondo = new CCSprite ("blocnotas");
			fondo.ScaleX = Escala.X;
			fondo.ScaleY = Escala.Y;
			AddChild (fondo);
		}



		//Método en el que añadir elementos a nuestra escena
        protected override void AddedToScene ()
        {
            base.AddedToScene ();


			fondo.Position = VisibleBoundsWorldspace.Center;



			//preparamos una etiqueta y configuramos sus propiedades
			var label1 = new CCLabelTtf("Lista de la compra:", "Handscript", 40) {
				Position = VisibleBoundsWorldspace.Center.Offset(-380*Escala.X,455*Escala.Y),
				Color = CCColor3B.Black,

				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			var label2 = new CCLabelTtf("Nivel:", "Handscript", 40) {
				Position = VisibleBoundsWorldspace.Center.Offset(500*Escala.X,455*Escala.Y),
				Color = CCColor3B.Black,

				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			var label3 = new CCLabelTtf(NumProductos.ToString(), "Handscript", 40) {
				Position = VisibleBoundsWorldspace.Center.Offset(600*Escala.X,455*Escala.Y),
				Color = CCColor3B.Black,

				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};


			var label4 = new CCLabelTtf("Pulsa la pantalla.", "Handscript", 40) {
				Position = VisibleBoundsWorldspace.Center.Offset(0,-420*Escala.Y),
				Color = CCColor3B.Black,

				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				Dimensions=new CCSize(900*Escala.X,100*Escala.Y),
				AnchorPoint = CCPoint.AnchorMiddle};

			float y = 341*Escala.Y;

			foreach (var pr1 in listaCompra) {
				var label = new CCLabelTtf(pr1.nombre, "Handscript", 22) {
					Position = VisibleBoundsWorldspace.Center.Offset(0,y),
					Color = CCColor3B.Black,

					HorizontalAlignment = CCTextAlignment.Center,
					VerticalAlignment = CCVerticalTextAlignment.Center,
					AnchorPoint = CCPoint.AnchorMiddle


				};

				//La añadimos a nuestra escena.
				AddChild (label4);
				AddChild (label3);
				AddChild (label2);
				AddChild (label1);
				AddChild (label);
				y = y - 57*Escala.Y;
			}
		}

		//Función llamada desde el Aplicationdelegate y que devuelve la escena.
		public static CCScene GameStartLayerScene(CCWindow mainWindow, int s)
        {
            var scene = new CCScene (mainWindow);
            var layer = new GameStartLayer (s);

            scene.AddChild (layer);

            return scene;
        }
    }
}