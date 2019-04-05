using Android.Runtime;
using Android.Views;
using Mobile.Droid.Helpers;
using Mobile.Helpers;
using System.ComponentModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using AView = Android.Views;

[assembly: ExportRenderer(typeof(DraggableView), typeof(DraggableViewRenderer))]
namespace Mobile.Droid.Helpers
{
#pragma warning disable CS0618 // Type or member is obsolete
    public class DraggableViewRenderer : VisualElementRenderer<Xamarin.Forms.View>
    {
        private float originalX;
        private float originalY;
        private float dX;
        private float dY;
        private bool firstTime = true;
        private bool touchedDown = false;

        protected override void OnElementChanged(ElementChangedEventArgs<Xamarin.Forms.View> e)
        {
            base.OnElementChanged(e);


            if (e.OldElement != null)
            {
                LongClick -= HandleLongClick;
            }
            if (e.NewElement != null)
            {
                LongClick += HandleLongClick;
                DraggableView dragView = Element as DraggableView;
                dragView.RestorePositionCommand = new Command(() =>
                {
                    SetX(originalX);
                    SetY(originalY);
                    //if (!firstTime)
                    //{
                    //    SetX(originalX);
                    //    SetY(originalY);
                    //}

                });
            }

        }

        private void HandleLongClick(object sender, LongClickEventArgs e)
        {
            DraggableView dragView = Element as DraggableView;
            if (firstTime)
            {
                originalX = GetX();
                originalY = GetY();
                firstTime = false;
            }
            dragView.DragStarted();
            touchedDown = true;
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DraggableView dragView = Element as DraggableView;
            base.OnElementPropertyChanged(sender, e);

        }
        protected override void OnVisibilityChanged(AView.View changedView, [GeneratedEnum] ViewStates visibility)
        {
            base.OnVisibilityChanged(changedView, visibility);
            if (visibility == ViewStates.Visible)
            {



            }
        }
        public override bool OnTouchEvent(MotionEvent e)
        {
            float x = e.RawX;
            float y = e.RawY;
            DraggableView dragView = Element as DraggableView;
            switch (e.Action)
            {
                case MotionEventActions.Down:
                    if (dragView.DragMode == DragMode.Touch)
                    {
                        if (!touchedDown)
                        {
                            if (firstTime)
                            {
                                originalX = GetX();
                                originalY = GetY();
                                firstTime = false;
                            }
                            dragView.DragStarted();
                        }

                        touchedDown = true;
                    }
                    dX = x - this.GetX();
                    dY = y - this.GetY();
                    break;
                case MotionEventActions.Move:
                    if (touchedDown)
                    {
                        if (dragView.DragDirection == DragDirectionType.All || dragView.DragDirection == DragDirectionType.Horizontal)
                        {
                            SetX(x - dX);
                        }

                        if (dragView.DragDirection == DragDirectionType.All || dragView.DragDirection == DragDirectionType.Vertical)
                        {
                            SetY(y - dY);
                        }
                    }
                    dragView.DragEnded();
                    break;
                case MotionEventActions.Up:
                    touchedDown = false;
                    dragView.DragEnded();
                    break;
                case MotionEventActions.Cancel:
                    touchedDown = false;
                    break;
            }
            return base.OnTouchEvent(e);
        }

        public override bool OnInterceptTouchEvent(MotionEvent e)
        {

            BringToFront();
            return true;
        }

    }
#pragma warning restore CS0618 // Type or member is obsolete
}