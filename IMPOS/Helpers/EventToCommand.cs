using System;
using System.Windows;
using System.Windows.Input;
using System.Reflection;
using System.Diagnostics;
using System.Windows.Interactivity;

namespace IMPOS.Helpers
{

    /// <summary>
    /// tabdil event be command haye mvvm
    /// drag and drop
    /// </summary>
    public class CommandExecuter
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached("Command", typeof(ICommand), typeof(CommandExecuter), new PropertyMetadata(CommandPropertyChangedCallback));

        public static readonly DependencyProperty OnEventProperty = DependencyProperty.RegisterAttached("OnEvent", typeof(string), typeof(CommandExecuter));

        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached("CommandParameter", typeof(object), typeof(CommandExecuter));

        public static void CommandPropertyChangedCallback(DependencyObject depObj, DependencyPropertyChangedEventArgs args)
        {
            string onEvent = (string)depObj.GetValue(OnEventProperty);
            Debug.Assert(onEvent != null, "OnEvent must be set.");
            var eventInfo = depObj.GetType().GetEvent(onEvent);
            if (eventInfo != null)
            {
                var mInfo = typeof(CommandExecuter).GetMethod("OnRoutedEvent", BindingFlags.NonPublic | BindingFlags.Static);
                eventInfo.GetAddMethod().Invoke(depObj, new object[] { Delegate.CreateDelegate(eventInfo.EventHandlerType, mInfo) });
            }
            else
            {
                Debug.Fail(string.Format("{0} is not found on object {1}", onEvent, depObj.GetType()));

            }

        }
        public static ICommand GetCommand(UIElement element)
        {
            return (ICommand)element.GetValue(CommandProperty);
        }
        public static void SetCommand(UIElement element, ICommand command)
        {
            element.SetValue(CommandProperty, command);
        }
        public static string GetOnEvent(UIElement element)
        {
            return (string)element.GetValue(OnEventProperty);
        }
        public static void SetOnEvent(UIElement element, string evnt)
        {
            element.SetValue(OnEventProperty, evnt);
        }
        public static object GetCommandParameter(UIElement element)
        {
            return (object)element.GetValue(CommandParameterProperty);
        }
        public static void SetCommandParameter(UIElement element, object commandParam)
        {
            element.SetValue(CommandParameterProperty, commandParam);
        }
        private static void OnRoutedEvent(object sender, RoutedEventArgs e)
        {
            UIElement element = (UIElement)sender;
            if (element != null)
            {
                ICommand command = element.GetValue(CommandProperty) as ICommand;
                if (command != null && command.CanExecute(element.GetValue(CommandParameterProperty)))
                {
                    command.Execute(element.GetValue(CommandParameterProperty));
                }
            }
        }
    }

    public sealed class EventCommand : TriggerAction<DependencyObject>
    {

        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(EventCommand), null);


        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register(
            "Command", typeof(ICommand), typeof(EventCommand), null);


        public static readonly DependencyProperty InvokeParameterProperty = DependencyProperty.Register(
            "InvokeParameter", typeof(object), typeof(EventCommand), null);

        private string commandName;

        public object InvokeParameter
        {
            get
            {
                return this.GetValue(InvokeParameterProperty);
            }
            set
            {
                this.SetValue(InvokeParameterProperty, value);
            }
        }


        public ICommand Command
        {
            get
            {
                return (ICommand)this.GetValue(CommandProperty);
            }
            set
            {
                this.SetValue(CommandProperty, value);
            }
        }

        public string CommandName
        {
            get
            {
                return this.commandName;
            }
            set
            {
                if (this.CommandName != value)
                {
                    this.commandName = value;
                }
            }
        }

        public object CommandParameter
        {
            get
            {
                return this.GetValue(CommandParameterProperty);
            }

            set
            {
                this.SetValue(CommandParameterProperty, value);
            }
        }

        public object Sender { get; set; }

        protected override void Invoke(object parameter)
        {
            this.InvokeParameter = parameter;
            if (this.AssociatedObject != null)
            {
                ICommand command = this.Command;
                if ((command != null) && command.CanExecute(this.CommandParameter))
                {
                    command.Execute(this.CommandParameter);
                }
            }
        }
    }
}


//using System;
//using System.Windows.Input;
//using Windows.UI.Xaml;
//using Windows.UI.Xaml.Controls;
//using Windows.UI.Xaml.Controls.Primitives;
//using Windows.UI.Xaml.Input;

//namespace Win8Utils.Behaviors
//{
//    public static class EventToCommand
//    {
//        public enum EventKind
//        {
//            Tapped = 0,
//            TextChanged = 1,
//            SelectionChanged = 2,
//            None = -1,
//        }

//        /// <summary>
//        /// The CommandParameter attached property's name.
//        /// </summary>
//        public const string CommandParameterPropertyName = "CommandParameter";

//        /// <summary>
//        /// The Command attached property's name.
//        /// </summary>
//        public const string CommandPropertyName = "Command";

//        /// <summary>
//        /// The Event attached property's name.
//        /// </summary>
//        public const string EventPropertyName = "Event";

//        /// <summary>
//        /// Identifies the CommandParameter attached property.
//        /// </summary>
//        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
//            CommandParameterPropertyName,
//            typeof (object),
//            typeof (EventToCommand),
//            new PropertyMetadata(null));

//        /// <summary>
//        /// Identifies the Command attached property.
//        /// </summary>
//        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
//            CommandPropertyName,
//            typeof (ICommand),
//            typeof (EventToCommand),
//            new PropertyMetadata(
//                null));

//        /// <summary>
//        /// Identifies the Event attached property.
//        /// </summary>
//        public static readonly DependencyProperty EventProperty = DependencyProperty.RegisterAttached(
//            EventPropertyName,
//            typeof (EventKind),
//            typeof (EventToCommand),
//            new PropertyMetadata(
//                string.Empty,
//                (s, e) => AttachEvent(s, (EventKind)e.NewValue)));

//        /// <summary>
//        /// The PassEventArgs attached property's name.
//        /// </summary>
//        public const string PassEventArgsPropertyName = "PassEventArgs";

//        /// <summary>
//        /// Gets the value of the PassEventArgs attached property 
//        /// for a given dependency object.
//        /// </summary>
//        /// <param name="obj">The object for which the property value
//        /// is read.</param>
//        /// <returns>The value of the PassEventArgs property of the specified object.</returns>
//        public static bool GetPassEventArgs(DependencyObject obj)
//        {
//            return (bool)obj.GetValue(PassEventArgsProperty);
//        }

//        /// <summary>
//        /// Sets the value of the PassEventArgs attached property
//        /// for a given dependency object. 
//        /// </summary>
//        /// <param name="obj">The object to which the property value
//        /// is written.</param>
//        /// <param name="value">Sets the PassEventArgs value of the specified object.</param>
//        public static void SetPassEventArgs(DependencyObject obj, bool value)
//        {
//            obj.SetValue(PassEventArgsProperty, value);
//        }

//        /// <summary>
//        /// Identifies the PassEventArgs attached property.
//        /// </summary>
//        public static readonly DependencyProperty PassEventArgsProperty = DependencyProperty.RegisterAttached(
//            PassEventArgsPropertyName,
//            typeof(bool),
//            typeof(EventToCommand),
//            new PropertyMetadata(false));

//        /// <summary>
//        /// Gets the value of the Command attached property 
//        /// for a given dependency object.
//        /// </summary>
//        /// <param name="obj">The object for which the property value
//        /// is read.</param>
//        /// <returns>The value of the Command property of the specified object.</returns>
//        public static ICommand GetCommand(DependencyObject obj)
//        {
//            return (ICommand)obj.GetValue(CommandProperty);
//        }

//        /// <summary>
//        /// Gets the value of the CommandParameter attached property 
//        /// for a given dependency object.
//        /// </summary>
//        /// <param name="obj">The object for which the property value
//        /// is read.</param>
//        /// <returns>The value of the CommandParameter property of the specified object.</returns>
//        public static object GetCommandParameter(DependencyObject obj)
//        {
//            return obj.GetValue(CommandParameterProperty);
//        }

//        /// <summary>
//        /// Gets the value of the Event attached property 
//        /// for a given dependency object.
//        /// </summary>
//        /// <param name="obj">The object for which the property value
//        /// is read.</param>
//        /// <returns>The value of the Event property of the specified object.</returns>
//        public static EventKind GetEvent(DependencyObject obj)
//        {
//            return (EventKind)obj.GetValue(EventProperty);
//        }

//        /// <summary>
//        /// Sets the value of the Command attached property
//        /// for a given dependency object. 
//        /// </summary>
//        /// <param name="obj">The object to which the property value
//        /// is written.</param>
//        /// <param name="value">Sets the Command value of the specified object.</param>
//        public static void SetCommand(DependencyObject obj, ICommand value)
//        {
//            obj.SetValue(CommandProperty, value);
//        }

//        /// <summary>
//        /// Sets the value of the CommandParameter attached property
//        /// for a given dependency object. 
//        /// </summary>
//        /// <param name="obj">The object to which the property value
//        /// is written.</param>
//        /// <param name="value">Sets the CommandParameter value of the specified object.</param>
//        public static void SetCommandParameter(DependencyObject obj, object value)
//        {
//            obj.SetValue(CommandParameterProperty, value);
//        }

//        /// <summary>
//        /// Sets the value of the Event attached property
//        /// for a given dependency object. 
//        /// </summary>
//        /// <param name="obj">The object to which the property value
//        /// is written.</param>
//        /// <param name="value">Sets the Event value of the specified object.</param>
//        public static void SetEvent(DependencyObject obj, EventKind value)
//        {
//            obj.SetValue(EventProperty, value);
//        }

//        private static void AttachEvent(DependencyObject owner, EventKind kind)
//        {
//            var sender = owner as UIElement;
//            if (sender == null)
//            {
//                return;
//            }

//            switch (kind)
//            {
//                case EventKind.Tapped:
//                    sender.AddHandler(UIElement.TappedEvent, (TappedEventHandler)ExecuteCommand, true);
//                    break;

//                case EventKind.TextChanged:
//                    var text = sender as TextBox;
//                    if (text != null)
//                    {
//                        text.TextChanged += TextChanged;
//                    }
//                    break;

//                case EventKind.SelectionChanged:

//                    var control = sender as Selector;
//                    if (control != null)
//                    {
//                        control.SelectionChanged += ControlSelectionChanged;
//                    }

//                    break;

//                default:
//                    throw new NotSupportedException("Unsupported event");
//            }
//        }

//        private static void TextChanged(object s, TextChangedEventArgs e)
//        {
//            var sender = s as TextBox;
//            if (sender == null)
//            {
//                return;
//            }

//            ICommand command = GetCommand(sender);
//            if (command != null)
//            {
//                object parameter = GetCommandParameter(sender);
//                command.Execute(parameter);
//            }
//        }

//        static void ControlSelectionChanged(object s, SelectionChangedEventArgs e)
//        {
//            var sender = s as UIElement;
//            if (sender == null)
//            {
//                return;
//            }

//            var command = GetCommand(sender);
//            if (command != null)
//            {
//                if (GetPassEventArgs(sender))
//                {
//                    command.Execute(e);
//                }
//                else
//                {
//                    var parameter = GetCommandParameter(sender);
//                    command.Execute(parameter);
//                }
//            }
//        }

//        private static void ExecuteCommand(object s, RoutedEventArgs e)
//        {
//            var sender = s as UIElement;
//            if (sender == null)
//            {
//                return;
//            }

//            var command = GetCommand(sender);
//            if (command != null)
//            {
//                if (GetPassEventArgs(sender))
//                {
//                    command.Execute(e);
//                }
//                else
//                {
//                    var parameter = GetCommandParameter(sender);
//                    command.Execute(parameter);
//                }
//            }
//        }
//    }
//}
