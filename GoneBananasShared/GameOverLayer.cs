using System;
using System.Collections.Generic;
using CocosSharp;

namespace GoneBananas
{
    public class GameOverLayer : CCLayerColor
    {
		public CCPoint Escala;
        string scoreMessage = string.Empty;
		int vidas,score;
		float tiemporestante;
		CCSprite imagen;

		public GameOverLayer (int s, int v, int p, float t)
        {
			Escala = GoneBananasApplicationDelegate.scale;

			vidas = v;
			score = s;

			tiemporestante = t;

           
			//Con estos if comprobamos si el usuario se ha pasado el nivel o por el contrario ha fallado.
			//Dependiendo del resultado, mostramos uno u otro mensaje.
			if ((vidas == 0) || (tiemporestante < 1)) {
				var touchListener = new CCEventListenerTouchAllAtOnce ();
				touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameStartLayer.GameStartLayerScene (Window, GameStartLayer.NumProductos));

				AddEventListener (touchListener, this);
				scoreMessage = String.Format ("Fin de partida. Conseguiste comprar {0} productos.", score);
			} else {
				var touchListener = new CCEventListenerTouchAllAtOnce ();
				touchListener.OnTouchesEnded = (touches, ccevent) => Window.DefaultDirector.ReplaceScene (GameStartLayer.GameStartLayerScene (Window, score+1));

				AddEventListener (touchListener, this);
				scoreMessage = String.Format ("Felicidades, Conseguiste comprar {0} productos!", score);
			}
          

            Color = new CCColor3B (CCColor4B.Black);

            Opacity = 255;
        }

		//Con estos if comprobamos si el usuario se ha pasado el nivel o por el contrario ha fallado.
		//Dependiendo del resultado, mostramos uno u otro símbolo.
        public void AddSimbolo ()
        {
			if ((vidas == 0) || (tiemporestante < 1)) {
				imagen = new CCSprite ("error");
			} else {
				imagen = new CCSprite ("ok ");
			}
			imagen.Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X + 20, VisibleBoundsWorldspace.Size.Center.Y + 300*Escala.Y);
			imagen.Scale = 0.5f*Escala.Y;
			AddChild (imagen);

        }

        protected override void AddedToScene ()
        {
            base.AddedToScene ();

            Scene.SceneResolutionPolicy = CCSceneResolutionPolicy.ShowAll;

            var scoreLabel = new CCLabelTtf (scoreMessage, "Handscript", 22) {
				Position = new CCPoint (VisibleBoundsWorldspace.Size.Center.X, VisibleBoundsWorldspace.Size.Center.Y + 50*Escala.Y),
                Color = new CCColor3B (CCColor4B.Yellow),
                HorizontalAlignment = CCTextAlignment.Center,
                VerticalAlignment = CCVerticalTextAlignment.Center,
                AnchorPoint = CCPoint.AnchorMiddle
            };

            AddChild (scoreLabel);

			var playAgainLabel = new CCLabelTtf ();
			if ((vidas == 0) || (tiemporestante < 1)) {

				playAgainLabel = new CCLabelTtf ("Toca la pantalla para intentarlo de nuevo", "Handscript", 22) {
					Position = VisibleBoundsWorldspace.Size.Center,
					Color = new CCColor3B (CCColor4B.Green),
					HorizontalAlignment = CCTextAlignment.Center,
					VerticalAlignment = CCVerticalTextAlignment.Center,
					AnchorPoint = CCPoint.AnchorMiddle,
				};
			} else {

				playAgainLabel = new CCLabelTtf ("Toca la pantalla para pasar de nivel.", "Handscript", 22) {
					Position = VisibleBoundsWorldspace.Size.Center,
					Color = new CCColor3B (CCColor4B.Green),
					HorizontalAlignment = CCTextAlignment.Center,
					VerticalAlignment = CCVerticalTextAlignment.Center,
					AnchorPoint = CCPoint.AnchorMiddle,
				};
			}

           
         AddChild (playAgainLabel);

            AddSimbolo ();
        }



		public static CCScene SceneWithScore (CCWindow mainWindow, float tiemporestante, int vidas, int score, int puntuacion)
        {
            var scene = new CCScene (mainWindow);
			var layer = new GameOverLayer (score,vidas,puntuacion,tiemporestante);

            scene.AddChild (layer);

            return scene;
        }
    }
}