using GalaSoft.MvvmLight.Messaging;
using HelixToolkit.Wpf.SharpDX;
using MachineElements.ViewModels.Extensions;
using MachineElements.ViewModels.Messages.Inserters;
using MachineElements.ViewModels.Messages.Panel;
using Point3D = System.Windows.Media.Media3D.Point3D;
using Vector3D = System.Windows.Media.Media3D.Vector3D;
using InsertMessage = MachineElements.ViewModels.Interfaces.Messages.Panel.InsertMessage;
using MachineElements.ViewModels.Interfaces;

namespace MachineElements.ViewModels.Inserters
{
    public class InserterViewModel : InserterBaseViewModel
    {
        public double Diameter { get; set; }

        public double Length { get; set; }

        public int LoaderLinkId { get; set; }

        public int DischargerLinkId { get; set; }

        public InserterViewModel() : base()
        {
            MessengerInstance.Register<HookLinkForManageInsertersMessage>(this, OnHookLinkForManageInsertersMessage);
            MessengerInstance.Register<DriveInserterByLinkMessage>(this, OnDriveInserterByLinkMessage);
        }

        private void OnHookLinkForManageInsertersMessage(HookLinkForManageInsertersMessage msg)
        {
            MessengerInstance.Send(new HookLinkByInserterMessage() { InserterId = InserterId, LinkId = LoaderLinkId });
            MessengerInstance.Send(new HookLinkByInserterMessage() { InserterId = InserterId, LinkId = DischargerLinkId });
        }

        private void OnDriveInserterByLinkMessage(DriveInserterByLinkMessage msg)
        {
            if ((msg.InserterId == InserterId) && IsFarwardStepProgress)
            {
                if (msg.LinkId == LoaderLinkId)
                {
                    LoadInserter(msg.Value);
                }
                else if (msg.LinkId == DischargerLinkId)
                {
                    DischargeInserter(msg.Value);
                }
            }
        }

        private void LoadInserter(bool value)
        {
            if (value)
            {
                var builder = new MeshBuilder();

                builder.AddCylinder(Position.ToVector3(), 
                                    (Position + Direction * Length).ToVector3(),
                                    Diameter / 2.0);

                var vm = new InserterObjectViewModel() { Geometry = builder.ToMesh(), Material = Material, Visible = true };
                Children.Add(vm);
            }
        }

        private void DischargeInserter(bool value)
        {
            if (value)
            {
                Children.Clear();

                if (_chainTransform == null) _chainTransform = this.GetChainTansform();

                var t = _chainTransform.Value;
                var p = t.Transform(Position);
                var d = t.Transform(Direction);

                Messenger.Default.Send(new GetPanelTransformMessage()
                {
                    SetData = (b, m) =>
                    {
                        if (b)
                        {
                            var invT = m;

                            invT.Invert();

                            Messenger.Default.Send(new InsertMessage()
                            {
                                InsertElement = CreateInsertElement(invT.Transform(p), invT.Transform(d))
                            });
                        }
                    }
                });
            }
        }

        private IMachineElementViewModel CreateInsertElement(Point3D position, Vector3D direction)
        {
            var builder = new MeshBuilder();

            builder.AddCylinder(position.ToVector3(),
                                (position + direction * Length).ToVector3(),
                                Diameter / 2.0);
            return new InserterObjectViewModel() { Geometry = builder.ToMesh(), Material = Material, Visible = true };
        }
    }
}
