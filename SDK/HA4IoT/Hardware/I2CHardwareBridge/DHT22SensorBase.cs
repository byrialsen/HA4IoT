﻿using System;
using HA4IoT.Contracts.Adapters;

namespace HA4IoT.Hardware.I2CHardwareBridge
{
    public abstract class DHT22SensorBase : ISensorAdapter
    {
        private readonly int _id;
        private readonly DHT22Accessor _dht22Accessor;
        private float _value;

        protected DHT22SensorBase(int id, DHT22Accessor dht22Accessor)
        {
            if (dht22Accessor == null) throw new ArgumentNullException(nameof(dht22Accessor));

            _id = id;
            _dht22Accessor = dht22Accessor;
            dht22Accessor.ValuesUpdated += (s, e) => UpdateValue();
        }

        public event EventHandler<SensorAdapterValueChangedEventArgs> ValueChanged;

        private void UpdateValue()
        {
            _value = GetValueInternal((byte)_id, _dht22Accessor);
            ValueChanged?.Invoke(this, new SensorAdapterValueChangedEventArgs(_value));
        }

        protected abstract float GetValueInternal(int sensorId, DHT22Accessor dht22Accessor);
    }
}