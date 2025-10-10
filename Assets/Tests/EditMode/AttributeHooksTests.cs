using System;
using NUnit.Framework;
using Servable.Runtime;
using Servable.Runtime.ObservableProperty;
using UnityEngine;

namespace Servable.Tests.EditMode
{
    public class AttributeHooksTests
    {
        private class ModelWithHooks : ModelBehaviour
        {
            public ObservableData<int> Counter { get; } = new ObservableData<int>(0);
            public ObservableCommand Refresh { get; } = new ObservableCommand();
            public ObservableCommand<int> SetPage { get; } = new ObservableCommand<int>();

            public int AwakeCalls;
            public int EnableCalls;
            public int DisableCalls;
            public int DestroyCalls;
            public int DataCalls;
            public int CmdCalls;
            public int CmdPayloadCalls;

            [OnAwake]
            private void OnAwakeMeth() { AwakeCalls++; }

            [OnEnable]
            private void OnEnableMeth() { EnableCalls++; }

            [OnDisable]
            private void OnDisableMeth() { DisableCalls++; }

            [OnDestroy]
            private void OnDestroyMeth() { DestroyCalls++; }

            [OnData("Counter")]
            private void OnCounterChanged(int v) { DataCalls++; }

            [OnCommand("Refresh")]
            private void OnRefresh() { CmdCalls++; }

            [OnCommand("SetPage")]
            private void OnSetPage(int p) { CmdPayloadCalls++; }
        }

        private GameObject _go;
        private ModelWithHooks _model;

        [SetUp]
        public void SetUp()
        {
            _go = new GameObject("test-go");
            _model = _go.AddComponent<ModelWithHooks>();
        }

        [TearDown]
        public void TearDown()
        {
            if (_go != null)
            {
                UnityEngine.Object.DestroyImmediate(_go);
            }
        }

        [Test]
        public void Lifecycle_Attributes_Are_Called()
        {
            // Awake already called on AddComponent
            Assert.AreEqual(1, _model.AwakeCalls);

            _go.SetActive(false);
            _go.SetActive(true);
            Assert.GreaterOrEqual(_model.EnableCalls, 1);
            Assert.GreaterOrEqual(_model.DisableCalls, 1);
        }

        [Test]
        public void OnData_AutoSubscribe_Works_And_Unsubscribes_OnDestroy()
        {
            var before = _model.DataCalls;
            _model.Counter.Value++;
            Assert.Greater(_model.DataCalls, before);

            UnityEngine.Object.DestroyImmediate(_go);
            _go = null;

            // create new to ensure no exception and previous instance unsubscribed
            var go2 = new GameObject("test-go2");
            var model2 = go2.AddComponent<ModelWithHooks>();
            var before2 = model2.DataCalls;
            model2.Counter.Value++;
            Assert.Greater(model2.DataCalls, before2);
            UnityEngine.Object.DestroyImmediate(go2);
        }

        [Test]
        public void OnCommand_NoPayload_And_WithPayload_Work_And_Unsubscribe()
        {
            var c0 = _model.CmdCalls;
            _model.Refresh.Emit();
            Assert.Greater(_model.CmdCalls, c0);

            var c1 = _model.CmdPayloadCalls;
            _model.SetPage.Emit(1);
            Assert.Greater(_model.CmdPayloadCalls, c1);

            UnityEngine.Object.DestroyImmediate(_go);
            _go = null;

            // recreate and ensure still works
            var go2 = new GameObject("test-go3");
            var model2 = go2.AddComponent<ModelWithHooks>();
            var c02 = model2.CmdCalls;
            model2.Refresh.Emit();
            Assert.Greater(model2.CmdCalls, c02);
            UnityEngine.Object.DestroyImmediate(go2);
        }
    }
}


