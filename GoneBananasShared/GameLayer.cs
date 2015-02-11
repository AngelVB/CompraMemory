using System;
using System.Collections.Generic;
using CocosDenshion;
using CocosSharp;
using System.Linq;
using Box2D.Common;
using Box2D.Dynamics;
using Box2D.Collision.Shapes;

namespace GoneBananas
{
	//Pantalla principal de juego
	public class GameLayer : CCLayerColor
	{
		//CONSTANTES -------------------------------------------------------------------

		CCPoint Escala;

		//Velocidad protagonista
		const float Carrito_SPEED = 20000.0f;

		//Duración del juego
		const float GAME_DURATION = 120.0f;
		// game ends after 60 seconds or when the Carrito hits a ball, whichever comes first

		//Número de enemigos
		const int MAX_NUM_BALLS = 5;

		// point to meter ratio for physics
		const int PTM_RATIO = 32;

		//Tiempo restante
		float elapsedTime = 120f;

		// (Elementos de la escena)-------------------------------------------------


		CCSprite Carrito;
		CCLabelTtf puntos, puntos1, reloj, reloj1,vidas, vidas1;

		// Llamada a la función que elimina el producto al llegar a la parte baja de la pantalla
		CCCallFuncN moveProductoComplete = new CCCallFuncN (node => node.RemoveFromParent ());
		List<CCSprite> visibleProductos;
		List<CCSprite> hitProductos;


		int Puntuacion = 0;
		int PuntosVida = 5;
		int Cogidos;

		//Animación protagonista

		CCCallFuncN walkAnimStop = new CCCallFuncN (node => node.StopAllActions ());

		// Fondo

		CCSprite Fondo;

		// Productos
		CCSpriteBatchNode productosBatch;
		CCTexture2D productosTexture;


		//Constructor
		public GameLayer ()
		{
			Escala = GoneBananasApplicationDelegate.scale;

			//Listener para controlar la pulsación en pantalla.
			var touchListener = new CCEventListenerTouchAllAtOnce ();

			//Listener que trabaja mientras movemos el dedo por pantalla
			touchListener.OnTouchesMoved = OnTouchesEnded;

			//Añade el evento
			AddEventListener (touchListener, this);

			//Color de fondo
			Color = new CCColor3B (CCColor4B.White);
			Opacity = 255;

			//Listas con el número de productos visibles y el número de productos cazadas.
			visibleProductos = new List<CCSprite> ();
			hitProductos = new List<CCSprite> ();

			Cogidos = 0;

			//Batch de productos
			productosBatch = new CCSpriteBatchNode ("products.plist", 100);
			productosTexture = productosBatch.Texture;
			AddChild (productosBatch, 1, 1);

			AddFondo ();
			AddPuntos ();
			AddFallos ();
			AddReloj ();
			AddCarrito ();

			StartScheduling ();

			CCSimpleAudioEngine.SharedEngine.PlayBackgroundMusic ("Sounds/ambiente", true);
		}

		//Código que se ejecuta en bucle y que gestiona la lógica del juego. Finalizará la ejecución al entrar en EndGame()
		void StartScheduling ()
		{
			Schedule (t => {
				visibleProductos.Add (AddProducto ());
		
				if (ShouldEndGame ()) {
					EndGame ();
				}
					
			}, 0.7f);


			//Reloj
			Schedule (t => {

				elapsedTime -= t;
				if(elapsedTime >=100){
					reloj.Text = elapsedTime.ToString ().Substring (0,3);
				}else{reloj.Text = elapsedTime.ToString ().Substring (0,2);}
				if (ShouldEndGame ()) {
					EndGame ();
				}

			}, 1.0f);

			//Comprobar colisiones
			Schedule (t => CheckCollision ());

		}
		//Añadimos fondo (png)
		void AddFondo ()
		{
			Fondo = new CCSprite ("supermarket");
			Fondo.ScaleX = Escala.X;
			Fondo.ScaleY = Escala.Y;
			AddChild (Fondo);
		}

		//Añadimos Reloj
		void AddReloj ()
		{
			reloj1 = new CCLabelTtf ("Tiempo:", "Handscript", 40) {
				//Position = VisibleBoundsWorldspace.UpperRight.Offset (100*Escala.X, -700*Escala.Y),
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			reloj = new CCLabelTtf (elapsedTime.ToString (), "Handscript", 40) {
				//Position = VisibleBoundsWorldspace.UpperRight.Offset (100*Escala.X, -700*Escala.Y),
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			AddChild (reloj1);
			AddChild (reloj);
		}


		//Añadimos puntos 
		void AddPuntos ()
		{
			puntos1 = new CCLabelTtf ("Puntuacion: ", "Handscript", 40) {
			//	Position = VisibleBoundsWorldspace.UpperRight.Offset (100*Escala.X, -200*Escala.Y),
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			puntos = new CCLabelTtf ("0", "Handscript", 40) {
			//	Position = VisibleBoundsWorldspace.UpperRight.Offset (100*Escala.X, -50*Escala.Y),
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			AddChild (puntos1);
			AddChild (puntos);
		}

		//Añadimos vidas restantes
		void AddFallos ()
		{
			vidas1 = new CCLabelTtf ("Vidas: ", "Handscript", 40) {
			//	Position = VisibleBoundsWorldspace.UpperRight.Offset (100*Escala.X, -200*Escala.Y),
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};
			vidas = new CCLabelTtf (PuntosVida.ToString (), "Handscript", 40) {
			//	Position = VisibleBoundsWorldspace.UpperRight.Offset (100*Escala.X, -100*Escala.Y),
				Color = CCColor3B.Red,
				HorizontalAlignment = CCTextAlignment.Center,
				VerticalAlignment = CCVerticalTextAlignment.Center,
				AnchorPoint = CCPoint.AnchorMiddle
			};

			AddChild (vidas1);
			AddChild (vidas);
		}

		//Añadimos carrito
		void AddCarrito ()
		{
			Carrito = new CCSprite ("carrito");
			Carrito.ScaleX= Escala.X;
			Carrito.ScaleY = Escala.Y;

			AddChild (Carrito);
		}

		//Productos
		CCSprite AddProducto ()
		{
			int idx = (CCRandom.GetRandomInt (0, GameStartLayer.listaProductos.Count - 1));

			var producto = new CCSprite (productosTexture, GameStartLayer.listaProductos [idx].TamanyoyPos);

			var p = GetRandomPosition (producto.ContentSize);
			producto.Position = p;
			producto.ScaleX = 0.3f*Escala.X;
			producto.ScaleY = 0.3f*Escala.Y;

			AddChild (producto);

			var moveProducto = new CCMoveTo (5.0f, new CCPoint (producto.Position.X, -300*Escala.Y));
			producto.RunActions (moveProducto, moveProductoComplete);

			return producto;
		}


		//Función para conseguir una posición aleatoria desde la que caerá el producto.
		CCPoint GetRandomPosition (CCSize spriteSize)
		{
			double rnd = CCRandom.NextDouble ();
			double randomX = (rnd > 0) ? rnd * VisibleBoundsWorldspace.Size.Width - spriteSize.Width / 2
                : spriteSize.Width / 2;
			return new CCPoint ((float)randomX, VisibleBoundsWorldspace.Size.Height - spriteSize.Height / 2);
		}


		//Función para controlar colisiones.
		void CheckCollision ()
		{
			foreach (var producto in visibleProductos) {
				bool hit = producto.BoundingBoxTransformedToParent.IntersectsRect (Carrito.BoundingBoxTransformedToParent);
				if (hit) {
					var myItem = GameStartLayer.listaProductos.FindIndex (item => item.TamanyoyPos.Equals (producto.TextureRectInPixels));

					//El producto está en la lista
					if (GameStartLayer.listaCompra.Contains (GameStartLayer.listaProductos [myItem])) {

						var Item = GameStartLayer.listaCompra.FindIndex (item => item.Equals ((GameStartLayer.listaProductos [myItem])));

						//El producto no ha sido cogido anteriormente
						if (GameStartLayer.listaCompra [Item].Cogido == false) {

							Cogidos = Cogidos + 1;
							Puntuacion = Puntuacion + 1;

							CCSimpleAudioEngine.SharedEngine.PlayEffect ("Sounds/Beep");
							//Marcamos el producto como cogido
							GameStartLayer.listaCompra [Item].Cogido = true;


							if (Cogidos == GameStartLayer.NumProductos) {
								EndGame ();
							} else {
								puntos.Text = Puntuacion.ToString ();
							}
						};
						hitProductos.Add (producto);
						Explode (producto.Position);
						producto.RemoveFromParent ();

					//El producto no está en la lista
					} else {
						hitProductos.Add (producto);
						Puntuacion = Puntuacion - 1;
						PuntosVida = PuntosVida - 1;
						CCSimpleAudioEngine.SharedEngine.PlayEffect ("Sounds/Error");
						vidas.Text = PuntosVida.ToString ();
						Explode (producto.Position);
						producto.RemoveFromParent ();
						if (PuntosVida <= 0) {
							EndGame ();
						} else {
							puntos.Text = Puntuacion.ToString ();
						}
					}
				}
			}
				
			foreach (var producto in hitProductos) {

				visibleProductos.Remove (producto);
			}
		}

		void EndGame ()
		{
			// Paramos los bucles que controlan la lógica del juego.
			UnscheduleAll ();

			//Llamamos a la escena de Game Over  pasándole el tiempo restante, los puntos de vida, el número de productos cogidos y la puntuación.
			var gameOverScene = GameOverLayer.SceneWithScore (Window, elapsedTime, PuntosVida, Cogidos,Puntuacion);
			var transitionToGameOver = new CCTransitionMoveInR (0.3f, gameOverScene);

			Director.ReplaceScene (transitionToGameOver);
		}
						
		//Explosión de los productos. Partículas.
		void Explode (CCPoint pt)
		{
			var explosion = new CCParticleExplosion (pt); 
			explosion.TotalParticles = 10;
			explosion.AutoRemoveOnFinish = true;
			AddChild (explosion);
		}

		//Controla si se ha acabado el tiempo.
		bool ShouldEndGame ()
		{
			return elapsedTime < 0f;
		}
			
		//Movimiento del carrito.
		void OnTouchesEnded (List<CCTouch> touches, CCEvent touchEvent)
		{


			//Capturamos el punto tocado en pantalla.
			var location = touches [0].LocationOnScreen;
			location = WorldToScreenspace (location);
			Console.WriteLine (location.X);
			Console.WriteLine (location.Y);

			//Nos aseguramos de que tenemos el dedo sobre el carrito.
			bool hit = Carrito.BoundingBoxTransformedToWorld.ContainsPoint(location);


			if (hit) {

				location.Y = 70*Escala.Y;
				float ds = Math.Abs (Carrito.Position.X - location.X);

				var dt = ds / Carrito_SPEED;

				//Movemos el mono hasta el punto tocado
				var moveCarrito = new CCMoveTo (dt, location);

				Carrito.RunActions (moveCarrito, walkAnimStop);
			}
		}

		//Configuración general de la escena.
		protected override void AddedToScene ()
		{
			base.AddedToScene ();
			//Tipo de ventana.
			Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.NoBorder;

			//Posición del fondo
			Fondo.Position = VisibleBoundsWorldspace.Center.Offset (0, -100*Escala.Y);

			//Posición del carrito
			Carrito.Position = VisibleBoundsWorldspace.LowerLeft.Offset (70*Escala.X, 70*Escala.Y);

			var b = VisibleBoundsWorldspace;

			//Posicionamos los puntuadores...
			puntos1.Position = b.UpperRight.Offset (-450*Escala.X, -50*Escala.Y); //BUG: doesn't appear in visible area on Nexus 7 device
			puntos.Position = b.UpperRight.Offset (-180*Escala.X, -50*Escala.Y); //BUG: doesn't appear in visible area on Nexus 7 device

			vidas1.Position = b.UpperRight.Offset (-900*Escala.X, -50*Escala.Y); //BUG: doesn't appear in visible area on Nexus 7 device
			vidas.Position = b.UpperRight.Offset (-750*Escala.X, -50*Escala.Y); //BUG: doesn't appear in visible area on Nexus 7 device

			reloj1.Position = b.UpperRight.Offset (-1750*Escala.X, -50*Escala.Y); //BUG: doesn't appear in visible area on Nexus 7 device

			reloj.Position = b.UpperRight.Offset (-1600*Escala.X, -50*Escala.Y); //BUG: doesn't appear in visible area on Nexus 7 device
		}

		public override void OnEnter ()
		{
			base.OnEnter ();
		}

		public static CCScene GameScene (CCWindow mainWindow)
		{
			var scene = new CCScene (mainWindow);
			var layer = new GameLayer ();
			scene.AddChild (layer);
			return scene;
		}
	}
}