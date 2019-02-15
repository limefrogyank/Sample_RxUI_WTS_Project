using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

using ReactiveUI;

using Sample_RxUI_WTS_Project.Services;
using Sample_RxUI_WTS_Project.Views;

using Splat;

namespace Sample_RxUI_WTS_Project.ViewModels
{
    [Windows.UI.Xaml.Data.Bindable]
    public class ViewModelLocator
    {
        private static ViewModelLocator _current;
        public static ViewModelLocator Current => _current ?? (_current = new ViewModelLocator());


        public ViewModelLocator()
        {
            Locator.CurrentMutable.InitializeSplat();
            Locator.CurrentMutable.InitializeReactiveUI();

            Locator.CurrentMutable.RegisterConstant<NavigationServiceEx>(new NavigationServiceEx());
            Locator.CurrentMutable.RegisterConstant<ShellViewModel>(new ShellViewModel());
            Register<MainViewModel, MainPage>();
            Register<DynamicDataViewModel, DynamicDataPage>();
            Register<SecondBlankViewModel, SecondBlankPage>();
        }

        public SecondBlankViewModel SecondBlankViewModel => Locator.Current.GetService<SecondBlankViewModel>();

        public DynamicDataViewModel DynamicDataViewModel => Locator.Current.GetService<DynamicDataViewModel>();

        public MainViewModel MainViewModel => Locator.Current.GetService<MainViewModel>();

        public ShellViewModel ShellViewModel => Locator.Current.GetService<ShellViewModel>(); 

        public NavigationServiceEx NavigationService => Locator.Current.GetService<NavigationServiceEx>();


        public void Register<VM, V>()
            where VM : class
        {
            Locator.CurrentMutable.Register<VM>();

            //this is for using ReactiveUI's ViewModelViewControl for things like list items 
            Locator.CurrentMutable.Register<IViewFor<VM>>(() => (IViewFor<VM>)Activator.CreateInstance<V>());

            //this is for actual navigation
            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }

        public void RegisterConstant<VM, V>()
          where VM : class
        {
            Locator.CurrentMutable.RegisterConstant<VM>();

            //this is for using ReactiveUI's ViewModelViewControl for things like list items 
            Locator.CurrentMutable.Register<IViewFor<VM>>(() => (IViewFor<VM>)Activator.CreateInstance<V>());

            //this is for actual navigation
            NavigationService.Configure(typeof(VM).FullName, typeof(V));
        }
    }


    public static class LocatorEx
    {
        public static void Register<TConcrete>(this IMutableDependencyResolver resolver)
        {
            Func<IDependencyResolver, object> func = RegisterCache<TConcrete>.GetRegisterFunc();
            resolver.Register(() => func(resolver), typeof(TConcrete));
        }

        public static void RegisterConstant<TConcrete>(this IMutableDependencyResolver resolver)
        {
            Func<IDependencyResolver, object> func = RegisterCache<TConcrete>.GetRegisterFunc();
            resolver.RegisterConstant(func(resolver), typeof(TConcrete));
        }

        private static class RegisterCache<TConcrete>
        {
            private static Func<IDependencyResolver, object> cachedFunc;

            public static Func<IDependencyResolver, object> GetRegisterFunc()
            {
                return cachedFunc ?? (cachedFunc = GenerateFunc());
            }

            private static Func<IDependencyResolver, object> GenerateFunc()
            {
                ParameterExpression funcParameter = Expression.Parameter(typeof(IDependencyResolver));

                Type concreteType = typeof(TConcrete);

                // Must be a single constructor
                ConstructorInfo constructor =
                    //concreteType.GetTypeInfo().DeclaredConstructors.Single(x => x.GetParameters().Length > 0);
                    concreteType.GetTypeInfo().DeclaredConstructors.Single();

                MethodInfo getServiceMethodInfo =
                    typeof(IDependencyResolver).GetTypeInfo().GetDeclaredMethod("GetService");

                IList<Expression> parameterExpressions =
                    constructor.GetParameters().Select(
                        parameter =>
                            Expression.Convert(
                                Expression.Call(
                                    funcParameter,
                                    getServiceMethodInfo,
                                    Expression.Constant(parameter.ParameterType),
                                    Expression.Convert(Expression.Constant(null), typeof(string))),
                                parameter.ParameterType)).Cast<Expression>().ToList();

                NewExpression newValue = Expression.New(constructor, parameterExpressions);
                Expression converted = Expression.Convert(newValue, typeof(object));
                return Expression.Lambda<Func<IDependencyResolver, object>>(converted, funcParameter).Compile();
            }
        }
    }
}
