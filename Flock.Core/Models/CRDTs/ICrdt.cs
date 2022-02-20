namespace Flock.Core.Models.CRDTs {

    internal interface ICrdt<T> where T : ICrdt<T> {

        /// <summary>
        /// Merge another instance into this instance.
        /// </summary>
        void Merge(T other);
        
        T Copy();

    }
}
