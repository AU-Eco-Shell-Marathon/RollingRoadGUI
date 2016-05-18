using System;
using System.Collections.Generic;
using System.Linq;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using NUnit.Framework;
using RollingRoad.Core.ApplicationServices;
using RollingRoad.Core.DomainModel;
using RollingRoad.Core.DomainServices;
using RollingRoad.WinApplication.Dialogs;
using RollingRoad.WinApplication.ViewModels;

namespace RollingRoad.WinApplication.Test.Unit.ViewModels
{
    [TestFixture]
    public class DataSetsViewModelTests
    {
        private IOpenFileDialog _fileDialog;
        private IErrorMessageBox _errorMessageBox;
        private IDataSetDataFile _dataSetLoader;

        private IRepository<DataSet> _repository;
        private IUnitOfWork _unitOfWork; 

        private DataSetsViewModel _viewModel;

        [SetUp]
        public void SetUp()
        {
            _fileDialog = Substitute.For<IOpenFileDialog>();
            _errorMessageBox = Substitute.For<IErrorMessageBox>();
            _dataSetLoader = Substitute.For<IDataSetDataFile>();

            _repository = Substitute.For<IRepository<DataSet>>();
            _unitOfWork = Substitute.For<IUnitOfWork>();

            _viewModel = new DataSetsViewModel(_repository, _unitOfWork)
            {
                DataSetLoader = _dataSetLoader,
                OpenFileDialog = _fileDialog,
                ErrorMessageBox = _errorMessageBox
            };
        }

        private void SetupSubstitutesToReturnFileNameAndDataSet(DataSet set)
        {
            _fileDialog.ShowDialog().Returns(true);
            _fileDialog.FileName.Returns("TestFile1");

            _dataSetLoader.LoadFromFile(Arg.Any<string>()).Returns(set);
            _repository.Insert(Arg.Any<DataSet>()).Returns(set);
        }

        [Test]
        public void Ctor_RepostoryInserted_GetCalledOnce()
        {
            _repository.Received(1).Get();
        }

        [Test]
        public void ImportFromFileCommand_ExecuteCommand_ShowDialogCalledOnFileDialog()
        {
            _fileDialog.ShowDialog().Returns(false);

            _viewModel.ImportFromFileCommand.Execute(null);

            _fileDialog.Received(1).ShowDialog();
        }

        [TestCase("Testfile1.csv")]
        [TestCase("FileTest2.csv")]
        public void ImportFromFileCommand_DialogReturnsTrue_FileNameUsedInDataSetLoader(string filename)
        {
            _fileDialog.ShowDialog().Returns(true);
            _fileDialog.FileName.Returns(filename);
            
            _viewModel.ImportFromFileCommand.Execute(null);

            _dataSetLoader.Received(1).LoadFromFile(filename);
        }

        [Test]
        public void ImportFromFileCommand_DataSetReturned_InsertCalledWithDataSet()
        {
            DataSet testset1 = new DataSet();
            
            SetupSubstitutesToReturnFileNameAndDataSet(testset1);

            _viewModel.ImportFromFileCommand.Execute(null);

            _repository.Received(1).Insert(testset1);
        }

        [Test]
        public void ImportFromFileCommand_DataSetReturned_DataSetAddedToObservableCollection()
        {
            DataSet testset1 = new DataSet() {Description = "TestDescription"};

            SetupSubstitutesToReturnFileNameAndDataSet(testset1);

            _viewModel.ImportFromFileCommand.Execute(null);

            Assert.That(_viewModel.DataSets.First().Description, Is.EqualTo("TestDescription"));
        }

        [Test]
        public void ImportFromFileCommand_DataSetLoaderThrows_MessageBoxRequestedToShowWithExceptionMessage()
        {
            _fileDialog.ShowDialog().Returns(true);
            _fileDialog.FileName.Returns("TestFile1");

            _dataSetLoader.LoadFromFile(Arg.Any<string>()).Throws(new Exception("TestException"));

            _viewModel.ImportFromFileCommand.Execute(null);

            _errorMessageBox.Received(1).Show(Arg.Any<string>(), "TestException");
        }

        [Test]
        public void ImportFromFileCommand_DataSetReturned_SaveCalledOnUnitOfWork()
        {
            DataSet testset1 = new DataSet() { Description = "TestDescription" };

            SetupSubstitutesToReturnFileNameAndDataSet(testset1);

            _viewModel.ImportFromFileCommand.Execute(null);

            _unitOfWork.Received(1).Save();
        }

        [Test]
        public void ImportFromFileCommand_DataSetReturned_SaveCalledOnUnitOfWorkAfterInsert()
        {
            DataSet testset1 = new DataSet() { Description = "TestDescription" };

            SetupSubstitutesToReturnFileNameAndDataSet(testset1);
            _viewModel.ImportFromFileCommand.Execute(null);

            Received.InOrder(() =>
            {
                _repository.Received(1).Insert(Arg.Any<DataSet>());
                _unitOfWork.Received(1).Save();
            });

        }

        [TestCase("Testcase 1")]
        [TestCase("Casetest 2")]
        public void ImportFromFileCommand_ImportTwice_DataSetExists(string description)
        {
            DataSet testset1 = new DataSet() { Description = "Testcase 1" };
            DataSet testset2 = new DataSet() { Description = "Casetest 2" };

            SetupSubstitutesToReturnFileNameAndDataSet(testset1);
            _viewModel.ImportFromFileCommand.Execute(null);

            SetupSubstitutesToReturnFileNameAndDataSet(testset2);
            _viewModel.ImportFromFileCommand.Execute(null);

            Assert.That(_viewModel.DataSets.First(x => x.Description == description), Is.Not.Null);
        }

        [Test]
        public void RefreshCommand_Execute_GetCalledOnRepository()
        {
            _repository.ClearReceivedCalls();//Ignore the one the constuctor makes

            _viewModel.RefreshCommand.Execute(null);

            _repository.Received(1).Get();
        }

        [Test]
        public void RefreshCommand_ExecuteWithOneDatasetInRepository_DataSetPresentInDataSets()
        {
            DataSet testset1 = new DataSet() { Description = "Testcase 1" };
            ICollection<DataSet> sets = new List<DataSet>() {testset1};

            _repository.Get().Returns(sets);

            _viewModel.RefreshCommand.Execute(null);

            Assert.That(_viewModel.DataSets.First().Description, Is.EqualTo("Testcase 1"));
        }

        [TestCase(2)]
        [TestCase(10)]
        public void RefreshCommand_ExecuteWithMultipleDatasetsInRepository_DataSetPresentInDataSets(int count)
        {
            ICollection<DataSet> sets = new List<DataSet>() { };

            for (int i = 0; i < count; i++)
            {
                sets.Add(new DataSet() {Description = i.ToString()});
            }

            _repository.Get().Returns(sets);

            _viewModel.RefreshCommand.Execute(null);


            for (int i = 0; i < count; i++)
            {
                Assert.That(_viewModel.DataSets.First(x => x.Description == i.ToString()), Is.Not.Null);
            }
        }

        [Test]
        public void SelectionChangedCommand_NoDataSets_SelectedDataListsIsEmpty()
        {
            _viewModel.SelectionChanged.Execute(null);

            Assert.That(_viewModel.SelectedDatalists, Is.Empty);
        }

        [Test]
        public void SelectionChangedCommand_OneDataSetWithListsButNotSelected_SelecetdDataListsIsEmpty()
        {
            DataSetViewModel dataSetViewModel = new DataSetViewModel(
                new DataSet()
                {
                    DataLists = new List<DataList>()
                    {
                        new DataList("Name1", "Unit1"),
                        new DataList("Name2", "Unit2")
                    }
                });

            _viewModel.DataSets.Add(dataSetViewModel);

            _viewModel.SelectionChanged.Execute(null);

            Assert.That(_viewModel.SelectedDatalists, Is.Empty);
        }

        [Test]
        public void SelectionChangedCommand_OneDataSetWithOneSelectedList_SelectedListIsInSelected()
        {
            DataSetViewModel dataSetViewModel = new DataSetViewModel(
                new DataSet()
                {
                    DataLists = new List<DataList>()
                    {
                        new DataList("Name1", "Unit1"),
                        new DataList("Name2", "Unit2")
                    }
                });

            dataSetViewModel.Collection.First().Selected = true;
            dataSetViewModel.IsSelected = true;

            _viewModel.DataSets.Add(dataSetViewModel);

            _viewModel.SelectionChanged.Execute(null);

            Assert.That(_viewModel.SelectedDatalists.First().Name, Is.EqualTo("Name1"));
        }

        [Test]
        public void DataSetLoader_SetNull_ExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => _viewModel.DataSetLoader = null);
        }

        [Test]
        public void ErrorMessageBox_SetNull_ExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => _viewModel.ErrorMessageBox = null);
        }

        [Test]
        public void OpenFileDialog_SetNull_ExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => _viewModel.OpenFileDialog = null);
        }

        [Test]
        public void Ctor_RepositoryNull_ExceptionThrown()
        {
            Assert.Throws<ArgumentNullException>(() => new DataSetsViewModel(null, _unitOfWork));
        }

        [Test]
        public void ToString_NothingDone_DescriptiveText()
        {
            Assert.That(_viewModel.ToString(), Does.Contain("View"));
            Assert.That(_viewModel.ToString(), Does.Contain("Compare"));
        }
    }
}
