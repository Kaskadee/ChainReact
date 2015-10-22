namespace ChainReact.Utilities
{
    public class Trigger
    {
        private bool _value;
        private bool _flag;

        public Trigger(bool value)
        {
            Value = value;
        }

        public bool Value
        {
            get
            {
                var val = _value;
                _value = false;
                return val;
            }
            set
            {
                if (!_value && value && !_flag)
                {
                    _value = true;
                    _flag = true;
                    return;
                }
                if (_value && _flag)
                {
                    _value = false;
                    return;
                }
                if (_flag && !value)
                {
                    _flag = false;
                }
            }
        }

        public void Set(bool value)
        {
            _value |= value;
        }

        public void SetToFalse()
        {
            _value = false;
        }

        public static implicit operator bool (Trigger trigger)
        {
            return trigger.Value;
        }

        public static implicit operator Trigger(bool value)
        {
            return new Trigger(value);
        }

#if DEBUG
        public bool GetValue()
        {
            return _value;
        }
#endif
    }
}
