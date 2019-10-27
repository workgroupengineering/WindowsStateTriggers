// Copyright (c) Morten Nielsen. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Globalization;
using Windows.Foundation.Metadata;
using Windows.UI.Xaml;

namespace WindowsStateTriggers
{
	/// <summary>
	/// Enables a state if the value is equal to another value
	/// </summary>
	/// <remarks>
	/// <para>
	/// Example: Trigger if a value is null
	/// <code lang="xaml">
	///     &lt;triggers:EqualsStateTrigger Value="{Binding MyObject}" EqualTo="{x:Null}" />
	/// </code>
	/// </para>
	/// </remarks>
	public class EqualsStateTrigger : StateTriggerBase, ITriggerValue
	{
		private void UpdateTrigger()
		{
			IsActive = (EqualsStateTrigger.AreValuesEqual(Value, EqualTo, true));
		}

		/// <summary>
		/// Gets or sets the value for comparison.
		/// </summary>
		public object Value
		{
			get { return (object)GetValue(ValueProperty); }
			set { SetValue(ValueProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="Value"/> DependencyProperty
		/// </summary>
		public static readonly DependencyProperty ValueProperty =
			DependencyProperty.Register("Value", typeof(object), typeof(EqualsStateTrigger), 
			new PropertyMetadata(null, OnValuePropertyChanged));

		private static void OnValuePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var obj = (EqualsStateTrigger)d;
			obj.UpdateTrigger();
		}

		/// <summary>
		/// Gets or sets the value to compare equality to.
		/// </summary>
		public object EqualTo
		{
			get { return (object)GetValue(EqualToProperty); }
			set { SetValue(EqualToProperty, value); }
		}

		/// <summary>
		/// Identifies the <see cref="EqualTo"/> DependencyProperty
		/// </summary>
		public static readonly DependencyProperty EqualToProperty =
					DependencyProperty.Register("EqualTo", typeof(object), typeof(EqualsStateTrigger), new PropertyMetadata(null, OnValuePropertyChanged));


		internal static bool AreValuesEqual(object value1, object value2, bool convertType)
		{
            if (value1 is null && value2 is null)
            {
                return true;
            }
            else if (value1 is null || value2 is null)
            {
                return false;
            }
			else if (value1.Equals(value2) is var equalsResult
                && equalsResult)
			{
				return true;
			}
            else if (value1.GetType() == value2.GetType())
            {
                return equalsResult;
            }
			else if (convertType)
			{
				// Try the conversion in both ways:
				return ConvertTypeEquals(value1, value2) || ConvertTypeEquals(value2, value1);
			}
			return false;
		}

		private static bool ConvertTypeEquals(object value1, object value2)
		{
			// Let's see if we can convert:
			if (value2 is Enum)
			{
				value1 = ConvertToEnum(value2.GetType(), value1);
			}
			else
			{
				value1 = Convert.ChangeType(value1, value2.GetType(), CultureInfo.InvariantCulture);
			}
			return value2.Equals(value1);
		}

		private static object ConvertToEnum(Type enumType, object value)
		{
			try
			{
				return Enum.IsDefined(enumType, value) ? Enum.ToObject(enumType, value) : null;
			}
			catch
			{
				return null;
			}
		}

		#region ITriggerValue

		private bool m_IsActive;

		/// <summary>
		/// Gets a value indicating whether this trigger is active.
		/// </summary>
		/// <value><c>true</c> if this trigger is active; otherwise, <c>false</c>.</value>
		public bool IsActive
		{
			get { return m_IsActive; }
			private set
			{
				if (m_IsActive != value)
				{
					m_IsActive = value;
					base.SetActive(value);
					if (IsActiveChanged != null)
						IsActiveChanged(this, EventArgs.Empty);
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
