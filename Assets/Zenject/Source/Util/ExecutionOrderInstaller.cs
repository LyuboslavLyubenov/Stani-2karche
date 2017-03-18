namespace Zenject.Source.Util
{

    using System;
    using System.Collections.Generic;

    using Zenject.Source.Install;

    public class ExecutionOrderInstaller : Installer<List<Type>, ExecutionOrderInstaller>
    {
        List<Type> _typeOrder;

        public ExecutionOrderInstaller(List<Type> typeOrder)
        {
            this._typeOrder = typeOrder;
        }

        public override void InstallBindings()
        {
            // All tickables without explicit priorities assigned are given order of zero,
            // so put all of these before that (ie. negative)
            int order = -1 * this._typeOrder.Count;

            foreach (var type in this._typeOrder)
            {
                this.Container.BindExecutionOrder(type, order);
                order++;
            }
        }
    }
}

