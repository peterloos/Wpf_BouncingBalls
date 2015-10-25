using System;
using System.Windows;
using System.Collections.Generic;
using System.Threading;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Media;

namespace BouncingBallsEx
{
    public partial class MainWindow : Window
    {
        public static readonly int Delay = 5;
        public static readonly int MaxBalls = 50;

        // list of bouncing balls
        private List<BouncingBall> bouncingBalls;

        // threading utils
        private delegate void MoveBallsHandler (BouncingBall ball);
        private Thread bouncingThread;
        private bool stopped;

        private Random rand;

        public MainWindow()
        {
            this.InitializeComponent();

            this.bouncingBalls = new List<BouncingBall>();
        }

        private void BouncingWindow_Loaded(Object sender, RoutedEventArgs e)
        {
            // create single random generator for all balls
            this.rand = new Random();

            // add first ball
            BouncingBall ball = new BouncingBall(this.rand);
            this.bouncingBalls.Add(ball);
            this.BouncingCanvas.Children.Add(ball.Circle);

            // start bouncing thread
            this.bouncingThread = new Thread(this.BouncingThreadProc);
            this.stopped = false;
            this.bouncingThread.Start();
        }

        private void BouncingThreadProc()
        {
            MoveBallsHandler handler = new MoveBallsHandler(this.MoveBall);

            while (! this.stopped)
            {
                Monitor.Enter(this);
                for (int i = 0; i < this.bouncingBalls.Count; i++)
                    this.Dispatcher.BeginInvoke(handler, this.bouncingBalls[i]);
                Monitor.Exit(this);

                Thread.Sleep(MainWindow.Delay);
            }
        }

        private void MoveBall(BouncingBall ball)
        {
            // move ball
            ball.Left += ball.Direction.X;
            ball.Top += ball.Direction.Y;

            // check boundaries of window and remaining balls
            this.CheckBoundaries(ball);
            this.CheckCollisions(ball);
        }

        private void CheckBoundaries(BouncingBall ball)
        {
            if (ball.Center.Y < BouncingBall.Radius)
            {
                // top wall collision, invert direction vertical         
                if (ball.Direction.Y < 0)
                    ball.Direction = new Vector(ball.Direction.X, -ball.Direction.Y);
            }
            else if (ball.Center.Y > (this.BouncingCanvas.ActualHeight - BouncingBall.Radius))
            {
                // bottom wall collision, invert direction vertical                
                if (ball.Direction.Y > 0)
                    ball.Direction = new Vector(ball.Direction.X, -ball.Direction.Y);
            }
            else if (ball.Center.X < BouncingBall.Radius)
            {
                // left wall collision, invert direction horizontal
                if (ball.Direction.X < 0)
                    ball.Direction = new Vector(-ball.Direction.X, ball.Direction.Y);
            }
            else if (ball.Center.X > (this.BouncingCanvas.ActualWidth - BouncingBall.Radius))
            {
                // left wall collision, invert direction horizontal
                if (ball.Direction.X > 0)
                    ball.Direction = new Vector(-ball.Direction.X, ball.Direction.Y);
            }
        }

        private void CheckCollisions(BouncingBall ball)
        {
            bool colorswap = false;

            for (int i = 0; i < this.bouncingBalls.Count; i++)
            {
                if (Object.ReferenceEquals(ball, this.bouncingBalls[i]))
                    continue;

                Vector difference = ball.Center - this.bouncingBalls[i].Center;
                if (difference.Length <= (BouncingBall.Radius * 2))
                {
                    difference.Normalize();
                    ball.Direction += difference;
                    colorswap = true;
                }
            }

            ball.NormalizeDirection();

            if (colorswap)
            {
                if (ball.Circle.Stroke == Brushes.Blue)
                    ball.Circle.Stroke = Brushes.Red;
                else if (ball.Circle.Stroke == Brushes.Red)
                    ball.Circle.Stroke = Brushes.Green;
                else if (ball.Circle.Stroke == Brushes.Green)
                    ball.Circle.Stroke = Brushes.Yellow;
                else if (ball.Circle.Stroke == Brushes.Yellow)
                    ball.Circle.Stroke = Brushes.White;
                else if (ball.Circle.Stroke == Brushes.White)
                    ball.Circle.Stroke = Brushes.Blue;
            }
        }


        private void BouncingWindow_MouseDown(Object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                if (this.bouncingBalls.Count < MainWindow.MaxBalls)
                {
                    BouncingBall ball = new BouncingBall(this.rand);

                    ball.Left = e.GetPosition(this.BouncingCanvas).X - BouncingBall.Radius;
                    ball.Top = e.GetPosition(this.BouncingCanvas).Y - BouncingBall.Radius;

                    Monitor.Enter(this);
                    this.bouncingBalls.Add(ball);
                    this.BouncingCanvas.Children.Add(ball.Circle);
                    Monitor.Exit(this);
                }
            }
            else if (e.RightButton == MouseButtonState.Pressed)
            {
                int count = this.bouncingBalls.Count;
                if (count == 0)
                    return;

                Monitor.Enter(this);
                this.bouncingBalls.RemoveAt(count - 1);
                this.BouncingCanvas.Children.RemoveAt(count - 1);
                Monitor.Exit(this);
            }
        }

        private void BouncingWindow_Closing(Object sender, CancelEventArgs e)
        {
            this.stopped = true;
            this.bouncingThread.Join();
        }
    }
}
