using System;

namespace Flock.Core.Identification {

    /// <summary>
    /// Represents an identifier that contains several information like the time, and position on the Earth's surface.
    /// This has uniqueness properties, useful to create data without conflict.
    /// The internal logic also adds extra randomness to lower even more the collision chance
    /// when there is not enough precision in the positioning data.
    /// </summary>
    public class SpatialId {

        private readonly double _lat;
        private readonly double _lon;
        private readonly float _alt;
        private readonly long _clock;
        private readonly long _randomKey;
        
        /// <summary>
        /// Creates a unique value from the time and space location of a particle in the Earth's surface
        /// </summary>
        /// <param name="lat">The latitude in decimal degrees</param>
        /// <param name="lon">The longitude in decimal degrees</param>
        /// <param name="alt">The altitude in Meters from Mean Sea Level</param>
        /// <param name="clock">The ticks of the satellite clock</param>
        /// <param name="randomKey">Any random value to reduce probability of collision
        /// when the accuracy of the positioning data is low.</param>
        public SpatialId(double lat, double lon, float alt, long clock, long randomKey) {
            _lat = lat;
            _lon = lon;
            _alt = alt;
            _clock = clock;
            _randomKey = randomKey;
        }

        #region Equality members

        protected bool Equals(SpatialId other) {
            return _lat.Equals(other._lat) &&
                   _lon.Equals(other._lon) &&
                   _alt.Equals(other._alt) &&
                   _clock == other._clock &&
                   _randomKey == other._randomKey;
        }
        
        /// <inheritdoc />
        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((SpatialId) obj);
        }

        /// <inheritdoc />
        public override int GetHashCode() {
            return HashCode.Combine(_lat, _lon, _alt, _clock, _randomKey);
        }

        public static bool operator ==(SpatialId left, SpatialId right) {
            return Equals(left, right);
        }

        public static bool operator !=(SpatialId left, SpatialId right) {
            return !Equals(left, right);
        }

        #endregion
    }
}
