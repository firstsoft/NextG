using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LWIR.NET.Framework.MVVM
{
    /// <summary>
    /// Stores an <see cref="T:System.Action" /> without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    public class WeakAction
    {
        private Action _staticAction;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakAction's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (this._staticAction != null)
                {
                    return this._staticAction.Method.Name;
                }
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakAction's action's target.
        /// This is not necessarily the same as
        /// if the method is anonymous.
        /// </summary>
        protected WeakReference ActionReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakAction. This is not necessarily the same as
        /// if the method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the WeakAction is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return this._staticAction != null;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (this._staticAction == null && this.Reference == null)
                {
                    return false;
                }
                if (this._staticAction != null)
                {
                    return this.Reference == null || this.Reference.IsAlive;
                }
                return this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Action's owner. This object is stored as a 
        /// <see cref="T:System.WeakReference" />.
        /// </summary>
        public object Target
        {
            get
            {
                if (this.Reference == null)
                {
                    return null;
                }
                return this.Reference.Target;
            }
        }

        /// <summary>
        /// The target of the weak reference.
        /// </summary>
        protected object ActionTarget
        {
            get
            {
                if (this.ActionReference == null)
                {
                    return null;
                }
                return this.ActionReference.Target;
            }
        }

        /// <summary>
        /// Initializes an empty instance of the class.
        /// </summary>
        protected WeakAction()
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action action)
            : this((action == null) ? null : action.Target, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action action)
        {
            if (action.Method.IsStatic)
            {
                this._staticAction = action;
                if (target != null)
                {
                    this.Reference = new WeakReference(target);
                }
                return;
            }
            this.Method = action.Method;
            this.ActionReference = new WeakReference(action.Target);
            this.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        public void Execute()
        {
            if (this._staticAction != null)
            {
                this._staticAction();
                return;
            }
            object actionTarget = this.ActionTarget;
            if (this.IsAlive && this.Method != null && this.ActionReference != null && actionTarget != null)
            {
                this.Method.Invoke(actionTarget, null);
            }
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            this.Reference = null;
            this.ActionReference = null;
            this.Method = null;
            this._staticAction = null;
        }
    }

    /// <summary>
    /// Stores an Action without causing a hard reference
    /// to be created to the Action's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Action's parameter.</typeparam>
    public class WeakAction<T> : WeakAction, IExecuteWithObject
    {
        private Action<T> _staticAction;

        /// <summary>
        /// Gets the name of the method that this WeakAction represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (this._staticAction != null)
                {
                    return this._staticAction.Method.Name;
                }
                return base.Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Action's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (this._staticAction == null && base.Reference == null)
                {
                    return false;
                }
                if (this._staticAction != null)
                {
                    return base.Reference == null || base.Reference.IsAlive;
                }
                return base.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeakAction class.
        /// </summary>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(Action<T> action)
            : this((action == null) ? null : action.Target, action)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakAction class.
        /// </summary>
        /// <param name="target">The action's owner.</param>
        /// <param name="action">The action that will be associated to this instance.</param>
        public WeakAction(object target, Action<T> action)
        {
            if (action.Method.IsStatic)
            {
                this._staticAction = action;
                if (target != null)
                {
                    base.Reference = new WeakReference(target);
                }
                return;
            }
            base.Method = action.Method;
            base.ActionReference = new WeakReference(action.Target);
            base.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive. The action's parameter is set to default(T).
        /// </summary>
        public new void Execute()
        {
            this.Execute(default(T));
        }

        /// <summary>
        /// Executes the action. This only happens if the action's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        public void Execute(T parameter)
        {
            if (this._staticAction != null)
            {
                this._staticAction(parameter);
                return;
            }
            object actionTarget = base.ActionTarget;
            if (this.IsAlive && base.Method != null && base.ActionReference != null && actionTarget != null)
            {
                base.Method.Invoke(actionTarget, new object[]
				{
					parameter
				});
            }
        }

        /// <summary>
        /// Executes the action with a parameter of type object. This parameter
        /// will be casted to T. This method implements
        /// and can be useful if you store multiple WeakAction{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the action after
        /// being casted to T.</param>
        public void ExecuteWithObject(object parameter)
        {
            T parameterCasted = (T)((object)parameter);
            this.Execute(parameterCasted);
        }

        /// <summary>
        /// Sets all the actions that this WeakAction contains to null,
        /// which is a signal for containing objects that this WeakAction
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            this._staticAction = null;
            base.MarkForDeletion();
        }
    }
}
