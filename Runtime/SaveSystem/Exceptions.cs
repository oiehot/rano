using System;

namespace Rano.SaveSystem
{
    public class NotFoundDataException : Exception
    {
        public NotFoundDataException()
        {
        }

        public NotFoundDataException(string message)
            : base(message)
        {
        }

        public NotFoundDataException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
    public class StateValidateException : Exception
    {
        public StateValidateException()
        {
        }

        public StateValidateException(string message)
            : base(message)
        {
        }

        public StateValidateException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
    
    #if false
    public class GameObjectDataNotFoundException : Exception
    {
        public string SaveableEntityId { get; private set; }

        public GameObjectDataNotFoundException(string saveableEntityId)
        {
            SaveableEntityId = saveableEntityId;
        }
    }

    public class ComponentDataNotFoundException : Exception
    {
        public string SaveableEntityId { get; private set; }
        public string SaveableComponentName { get; private set; }

        public ComponentDataNotFoundException(string saveableEntityId, string saveableComponentName)
        {
            SaveableEntityId = saveableEntityId;
            SaveableComponentName = saveableComponentName;
        }
    }
    #endif

}