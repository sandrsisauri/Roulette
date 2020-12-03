using System;

namespace Roulette.Entity
{
    public class BaseEntity<T> where T : IComparable
    {
        public T Id { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? ModifiedAt { get; set; }
    }
}
