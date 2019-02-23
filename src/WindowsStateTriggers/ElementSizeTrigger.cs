﻿// Copyright (c) Morten Nielsen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Windows.UI.Xaml;

namespace WindowsStateTriggers
{
    public class ElementSizeTrigger : StateTriggerBase, ITriggerValue
    {
        public static readonly DependencyProperty ElementProperty =
            DependencyProperty.Register(
                "Element",
                typeof(FrameworkElement),
                typeof(ElementSizeTrigger),
                new PropertyMetadata(null, OnUIElementPropertyChanged));

        public static readonly DependencyProperty MinHeightProperty =
            DependencyProperty.Register(
                "MinHeight",
                typeof(double),
                typeof(ElementSizeTrigger),
                new PropertyMetadata(0d, OnMinHeightOrWidthPropertyChanged));

        public static readonly DependencyProperty MinWidthProperty =
            DependencyProperty.Register(
                "MinWidth",
                typeof(double),
                typeof(ElementSizeTrigger),
                new PropertyMetadata(0d, OnMinHeightOrWidthPropertyChanged));

        public FrameworkElement Element
        {
            get => (FrameworkElement)GetValue(ElementProperty);
            set => SetValue(ElementProperty, value);
        }

        public double MinHeight
        {
            get => (double)GetValue(MinHeightProperty);
            set => SetValue(MinHeightProperty, value);
        }

        public double MinWidth
        {
            get => (double)GetValue(MinWidthProperty);
            set => SetValue(MinWidthProperty, value);
        }

        private static void OnMinHeightOrWidthPropertyChanged(DependencyObject dependencyObject, object _)
        {
            if (dependencyObject is ElementSizeTrigger trigger
                && trigger.Element is FrameworkElement element)
            {
                trigger.IsActive = element.ActualHeight >= trigger.MinHeight && element.ActualWidth >= trigger.MinWidth;
            }
        }

        private static void OnUIElementPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue is FrameworkElement element
                && d is ElementSizeTrigger trigger)
            {
                var weakEvent = new WeakEventListener<ElementSizeTrigger, object, SizeChangedEventArgs>(trigger)
                {
                    OnEventAction = OnElementSizeChanged,
                    OnDetachAction = (_, weakEventListener) => element.SizeChanged -= weakEventListener.OnEvent
                };
                element.SizeChanged += weakEvent.OnEvent;
            }
        }

        private static void OnElementSizeChanged(ElementSizeTrigger instance, object sender, SizeChangedEventArgs args)
        {
            instance.IsActive = args.NewSize.Height >= instance.MinHeight && args.NewSize.Width >= instance.MinWidth;
        }

        #region ITriggerValue

        private bool isActive;

        /// <summary>
        /// Gets a value indicating whether this trigger is active.
        /// </summary>
        /// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
        public bool IsActive
        {
            get => isActive;
            private set
            {
                if (isActive != value)
                {
                    isActive = value;
                    SetActive(value);
                    IsActiveChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        /// Occurs when the <see cref="IsActive" /> property has changed.
        /// </summary>
        public event EventHandler IsActiveChanged;

        #endregion ITriggerValue
    }
}
