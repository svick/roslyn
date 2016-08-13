﻿// Copyright (c) Microsoft.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Roslyn.Test.Performance.Utilities
{
    public interface ITraceManager
    {
        bool HasWarmUpIteration { get; }

        void Initialize();
        void Cleanup();
        void EndEvent();
        void EndScenario();
        void EndScenarios();
        void ResetScenarioGenerator();
        void Setup();
        void Start();
        void StartEvent();
        void StartScenario(string scenarioName, string processName);
        void StartScenarios();
        void Stop();
        void WriteScenarios(string[] scenarios);
        void WriteScenariosFileToDisk();
    }
}
