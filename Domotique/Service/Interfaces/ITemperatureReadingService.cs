﻿namespace Domotique.Service
{
    public interface ITemperatureReadingService
    {
        bool IsStarted { get; set; }

        void Start();
    }
}