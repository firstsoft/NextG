using LWIR.NET.Framework.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LWIR.NET.Framework.MVVM
{
    /// <summary>
    /// Stores a Func&lt;T&gt; without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="TResult">The type of the result of the Func that will be stored
    /// by this weak reference.</typeparam>
    public class WeakFunc<TResult>
    {
        private Func<TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the <see cref="T:System.Reflection.MethodInfo" /> corresponding to this WeakFunc's
        /// method passed in the constructor.
        /// </summary>
        protected MethodInfo Method
        {
            get;
            set;
        }

        /// <summary>
        /// Get a value indicating whether the WeakFunc is static or not.
        /// </summary>
        public bool IsStatic
        {
            get
            {
                return this._staticFunc != null;
            }
        }

        /// <summary>
        /// Gets the name of the method that this WeakFunc represents.
        /// </summary>
        public virtual string MethodName
        {
            get
            {
                if (this._staticFunc != null)
                {
                    return this._staticFunc.Method.Name;
                }
                return this.Method.Name;
            }
        }

        /// <summary>
        /// Gets or sets a WeakReference to this WeakFunc's action's target.
        /// This is not necessarily the same as
        /// if the method is anonymous.
        /// </summary>
        protected WeakReference FuncReference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets or sets a WeakReference to the target passed when constructing
        /// the WeakFunc. This is not necessarily the same as
        /// if the method is anonymous.
        /// </summary>
        protected WeakReference Reference
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public virtual bool IsAlive
        {
            get
            {
                if (this._staticFunc == null && this.Reference == null)
                {
                    return false;
                }
                if (this._staticFunc != null)
                {
                    return this.Reference == null || this.Reference.IsAlive;
                }
                return this.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Gets the Func's owner. This object is stored as a 
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
        /// Gets the owner of the Func that was passed as parameter.
        /// This is not necessarily the same as
        /// if the method is anonymous.
        /// </summary>
        protected object FuncTarget
        {
            get
            {
                if (this.FuncReference == null)
                {
                    return null;
                }
                return this.FuncReference.Target;
            }
        }

        /// <summary>
        /// Initializes an empty instance of the WeakFunc class.
        /// </summary>
        protected WeakFunc()
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<TResult> func)
            : this((func == null) ? null : func.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<TResult> func)
        {
            if (func.Method.IsStatic)
            {
                this._staticFunc = func;
                if (target != null)
                {
                    this.Reference = new WeakReference(target);
                }
                return;
            }
            this.Method = func.Method;
            this.FuncReference = new WeakReference(func.Target);
            this.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the action. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute()
        {
            if (this._staticFunc != null)
            {
                return this._staticFunc();
            }
            object funcTarget = this.FuncTarget;
            if (this.IsAlive && this.Method != null && this.FuncReference != null && funcTarget != null)
            {
                return (TResult)((object)this.Method.Invoke(funcTarget, null));
            }
            return default(TResult);
        }

        /// <summary>
        /// Sets the reference that this instance stores to null.
        /// </summary>
        public void MarkForDeletion()
        {
            this.Reference = null;
            this.FuncReference = null;
            this.Method = null;
            this._staticFunc = null;
        }
    }

    /// <summary>
    /// Stores an Func without causing a hard reference
    /// to be created to the Func's owner. The owner can be garbage collected at any time.
    /// </summary>
    /// <typeparam name="T">The type of the Func's parameter.</typeparam>
    /// <typeparam name="TResult">The type of the Func's return value.</typeparam>
    public class WeakFunc<T, TResult> : WeakFunc<TResult>, IExecuteWithObjectAndResult
    {
        private Func<T, TResult> _staticFunc;

        /// <summary>
        /// Gets or sets the name of the method that this WeakFunc represents.
        /// </summary>
        public override string MethodName
        {
            get
            {
                if (this._staticFunc != null)
                {
                    return this._staticFunc.Method.Name;
                }
                return base.Method.Name;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the Func's owner is still alive, or if it was collected
        /// by the Garbage Collector already.
        /// </summary>
        public override bool IsAlive
        {
            get
            {
                if (this._staticFunc == null && base.Reference == null)
                {
                    return false;
                }
                if (this._staticFunc != null)
                {
                    return base.Reference == null || base.Reference.IsAlive;
                }
                return base.Reference.IsAlive;
            }
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(Func<T, TResult> func)
            : this((func == null) ? null : func.Target, func)
        {
        }

        /// <summary>
        /// Initializes a new instance of the WeakFunc class.
        /// </summary>
        /// <param name="target">The Func's owner.</param>
        /// <param name="func">The Func that will be associated to this instance.</param>
        public WeakFunc(object target, Func<T, TResult> func)
        {
            if (func.Method.IsStatic)
            {
                this._staticFunc = func;
                if (target != null)
                {
                    base.Reference = new WeakReference(target);
                }
                return;
            }
            base.Method = func.Method;
            base.FuncReference = new WeakReference(func.Target);
            base.Reference = new WeakReference(target);
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive. The Func's parameter is set to default(T).
        /// </summary>
        /// <returns>The result of the Func stored as reference.</returns>
        public new TResult Execute()
        {
            return this.Execute(default(T));
        }

        /// <summary>
        /// Executes the Func. This only happens if the Func's owner
        /// is still alive.
        /// </summary>
        /// <param name="parameter">A parameter to be passed to the action.</param>
        /// <returns>The result of the Func stored as reference.</returns>
        public TResult Execute(T parameter)
        {
            if (this._staticFunc != null)
            {
                return this._staticFunc(parameter);
            }
            object funcTarget = base.FuncTarget;
            if (this.IsAlive && base.Method != null && base.FuncReference != null && funcTarget != null)
            {
                return (TResult)((object)base.Method.Invoke(funcTarget, new object[]
				{
					parameter
				}));
            }
            return default(TResult);
        }

        /// <summary>
        /// Executes the Func with a parameter of type object. This parameter
        /// will be casted to T. This method implements
        /// and can be useful if you store multiple WeakFunc{T} instances but don't know in advance
        /// what type T represents.
        /// </summary>
        /// <param name="parameter">The parameter that will be passed to the Func after
        /// being casted to T.</param>
        /// <returns>The result of the execution as object, to be casted to T.</returns>
        public object ExecuteWithObject(object parameter)
        {
            T parameterCasted = (T)((object)parameter);
            return this.Execute(parameterCasted);
        }

        /// <summary>
        /// Sets all the funcs that this WeakFunc contains to null,
        /// which is a signal for containing objects that this WeakFunc
        /// should be deleted.
        /// </summary>
        public new void MarkForDeletion()
        {
            this._staticFunc = null;
            base.MarkForDeletion();
        }
    }
}
