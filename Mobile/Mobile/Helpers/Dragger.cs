using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace Mobile.Helpers
{
    public partial class DraggableView : ContentView
    {
        public event EventHandler DragStart = delegate { };
        public event EventHandler DragEnd = delegate { };

        public static readonly BindableProperty DragDirectionProperty = BindableProperty.Create(
            propertyName: "DragDirection",
            returnType: typeof(DragDirectionType),
            declaringType: typeof(DraggableView),
            defaultValue: DragDirectionType.All,
            defaultBindingMode: BindingMode.TwoWay);

        public DragDirectionType DragDirection
        {
            get { return (DragDirectionType)GetValue(DragDirectionProperty); }
            set { SetValue(DragDirectionProperty, value); }
        }


        public static readonly BindableProperty DragModeProperty = BindableProperty.Create(
           propertyName: "DragMode",
           returnType: typeof(DragMode),
           declaringType: typeof(DraggableView),
           defaultValue: DragMode.LongPress,
           defaultBindingMode: BindingMode.TwoWay);

        public DragMode DragMode
        {
            get { return (DragMode)GetValue(DragModeProperty); }
            set { SetValue(DragModeProperty, value); }
        }

        public static readonly BindableProperty DragToProperty = BindableProperty.Create(
           propertyName: "DragTo",
           returnType: typeof(DragTo),
           declaringType: typeof(DraggableView),
           defaultValue: DragTo.LeftToRight,
           defaultBindingMode: BindingMode.TwoWay);

        public DragTo DragTo
        {
            get { return (DragTo)GetValue(DragToProperty); }
            set { SetValue(DragToProperty, value); }
        }

        public static readonly BindableProperty IsDraggingProperty = BindableProperty.Create(
          propertyName: "IsDragging",
          returnType: typeof(bool),
          declaringType: typeof(DraggableView),
          defaultValue: false,
          defaultBindingMode: BindingMode.TwoWay);

        public bool IsDragging
        {
            get { return (bool)GetValue(IsDraggingProperty); }
            set { SetValue(IsDraggingProperty, value); }
        }

        public static readonly BindableProperty IsDraggedProperty = BindableProperty.Create(
         propertyName: "IsDragged",
         returnType: typeof(bool),
         declaringType: typeof(DraggableView),
         defaultValue: false,
         defaultBindingMode: BindingMode.TwoWay);

        public bool IsDragged
        {
            get { return (bool)GetValue(IsDraggedProperty); }
            set { SetValue(IsDraggedProperty, value); }
        }

        public static readonly BindableProperty DragValueProperty = BindableProperty.Create(
        propertyName: "DragValue",
        returnType: typeof(double),
        declaringType: typeof(DraggableView),
        defaultValue: (double)0,
        defaultBindingMode: BindingMode.TwoWay);

        public double DragValue
        {
            get { return (double)GetValue(DragValueProperty); }
            set { SetValue(DragValueProperty, value); }
        }


        public static readonly BindableProperty RestorePositionCommandProperty = BindableProperty.Create(nameof(RestorePositionCommand), typeof(ICommand), typeof(DraggableView), default(ICommand), BindingMode.TwoWay, null, OnRestorePositionCommandPropertyChanged);

        static void OnRestorePositionCommandPropertyChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (!(bindable is DraggableView source))
            {
                return;
            }
            source.OnRestorePositionCommandChanged();
        }

        private void OnRestorePositionCommandChanged()
        {
            OnPropertyChanged("RestorePositionCommand");
        }

        public ICommand RestorePositionCommand
        {
            get
            {
                return (ICommand)GetValue(RestorePositionCommandProperty);
            }
            set
            {
                SetValue(RestorePositionCommandProperty, value);
            }
        }

        public void DragStarted()
        {
            DragStart(this, default(EventArgs));
            IsDragging = true;
        }

        public async void DragEnded(bool dragged, double value)
        {
            await Task.Delay(400);
            IsDragging = false;
            IsDragged = dragged;
            DragValue = value;
            DragEnd(this, default(EventArgs));
            RestorePositionCommand.Execute(null);

        }
      
    }
    public enum DragDirectionType
    {
        All,
        Vertical,
        Horizontal
    }
    public enum DragTo
    {
        RightToLeft,
        LeftToRight
    }

    public enum DragMode
    {
        Touch,
        LongPress
    }
}
