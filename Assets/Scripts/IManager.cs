using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FengSheng
{
    public interface IManager
    {
        public virtual void Register() { }
        public virtual void Unregister() { }
    }
}
