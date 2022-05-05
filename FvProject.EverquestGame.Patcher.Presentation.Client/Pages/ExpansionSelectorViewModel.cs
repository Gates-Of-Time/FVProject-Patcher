using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using FvProject.EverquestGame.Patcher.Application.Contracts;
using FvProject.EverquestGame.Patcher.Domain.Enums;
using FvProject.EverquestGame.Patcher.Presentation.Client.Events;
using Stylet;

namespace FvProject.EverquestGame.Patcher.Presentation.Client.Pages {
    public class ExpansionSelectorViewModel : Screen, IHandle<AvailableExpansionsEvent> {
        private int currentElement = 0;
        private readonly IEventAggregator _eventAggregator;
        private readonly IApplicationConfig _applicationConfig;

        public ExpansionSelectorViewModel(IEventAggregator eventAggregator, IApplicationConfig applicationConfig) {
            _eventAggregator = eventAggregator;
            _applicationConfig = applicationConfig;
            _eventAggregator?.Subscribe(this);
        }

        protected IDictionary<ExpansionsEnum, string> ExpansionImages { get; } = new Dictionary<ExpansionsEnum, string> {
            { ExpansionsEnum.Unknown, "/images/firionavie.jpg" },
            { ExpansionsEnum.Original, "/images/everquest-classic.jpg" },
            { ExpansionsEnum.RuinsOfKunark, "/images/everquest-kunark.jpg" },
            { ExpansionsEnum.ScarsOfVelious, "/images/everquest-velius.jpg" },
            { ExpansionsEnum.ShadowsOfLuclin, "/images/everquest-luclin.jpg" },
            { ExpansionsEnum.PlanesOfPower, "/images/everquest-power.jpg" },
        };

        private bool _canLeft = false;
        public bool CanLeft {
            get => _canLeft;
            set => SetAndNotify(ref _canLeft, value);
        }

        private bool _canRight = false;
        public bool CanRight {
            get => _canRight;
            set => SetAndNotify(ref _canRight, value);
        }

        private double _position = 0;
        public double Position {
            get => _position;
            set => SetAndNotify(ref _position, value);
        }

        private ObservableCollection<string> _expansions = new ObservableCollection<string>() { "/images/firionavie.jpg" };
        public ObservableCollection<string> Expansions {
            get => _expansions;
            set {
                SetAndNotify(ref _expansions, value);
                CanLeft = value.Count() > 1;
                CanRight = value.Count() > 1;
            }
        }

        public void Left() {
            if (currentElement > 0) {
                currentElement--;
            }
            else {
                currentElement = _expansions.Count() - 1;
            }

            Position = (-350) * currentElement;
            PublishExpansionSelectedEvent();
        }

        public void Right() {
            if (currentElement < _expansions.Count() - 1) {
                currentElement++;
            }
            else {
                currentElement = 0;
            }

            Position = (-350) * currentElement;
            PublishExpansionSelectedEvent();
        }

        private ExpansionsEnum SelectedExpansion {
            get {
                var selectedExpansion = Expansions[currentElement];
                return ExpansionImages.FirstOrDefault(x => x.Value == selectedExpansion).Key;
            }
        }

        #region Event aggregation
        private void PublishExpansionSelectedEvent() {
            _eventAggregator.Publish(new ExpansionSelectedEvent(SelectedExpansion));
        }

        public void Handle(AvailableExpansionsEvent message) {
            var patchableExpansionsImages = ExpansionImages.Where(x => message.ExpansionsFiles.ContainsKey(x.Key)).Select(x => x.Value);
            Expansions = new ObservableCollection<string>(patchableExpansionsImages);

            if (_applicationConfig.PreferredExpansion != null && ExpansionImages.ContainsKey(_applicationConfig.PreferredExpansion)) {
                currentElement = Expansions.IndexOf(ExpansionImages.First(x => x.Key == _applicationConfig.PreferredExpansion).Value);
                if (currentElement > 0) {
                    Position = (-350) * currentElement;
                }
                else {
                    Position = 0;
                }
            }

            PublishExpansionSelectedEvent();
        }
        #endregion Event aggregation
    }

    public class ExpansionSelectorDesignViewModel : ExpansionSelectorViewModel {
        public ExpansionSelectorDesignViewModel() : base(null, new ApplicationConfig()) {
            Expansions.Add("/images/everquest-classic.jpg");
        }
    }
}
