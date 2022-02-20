using System;
using Flock.Core.Identification;
using Flock.Core.Synchronization;

namespace Flock.Core.Models {

    public interface IModel<T> : IUnique where T : IModel<T> {

        event EventHandler StateChanged;

        IModelState GetState();

        void Merge(IModelState state);

    }
}
