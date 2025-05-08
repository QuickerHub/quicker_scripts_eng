using Jint;
using System;
using System.Text;

namespace ConsoleApp1.JintTest
{
    public class JintClassTest
    {
        public static void RunTest()
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.WriteLine("测试Jint对ES6类和构造函数的支持\n");
            
            // 创建一个新的Jint引擎实例
            var engine = new Engine();

            try
            {
                // 测试1: 基本类定义
                Console.WriteLine("测试1: 基本类定义");
                string basicClassResult = engine.Evaluate(@"
                    class Person1 {
                        sayHello() {
                            return 'Hello!';
                        }
                    }
                    
                    //const person = 
new Person1().sayHello();
                    //person.sayHello();
                ").ToString();
                
                Console.WriteLine($"结果: {basicClassResult}\n");

                // 测试2: 带构造函数的类
                Console.WriteLine("测试2: 带构造函数的类");
                string constructorResult = engine.Evaluate(@"
                    class Person2 {
                        constructor(name, age) {
                            this.name = name;
                            this.age = age;
                        }
                        
                        getInfo() {
                            return `Name: ${this.name}, Age: ${this.age}`;
                        }
                    }
                    
                    const person2 = new Person2('张三', 30);
                    person2.getInfo();
                ").ToString();
                
                Console.WriteLine($"结果: {constructorResult}\n");

                // 测试3: 类继承
                Console.WriteLine("测试3: 类继承");
                string inheritanceResult = engine.Evaluate(@"
                    class Animal {
                        constructor(name) {
                            this.name = name;
                        }
                        
                        speak() {
                            return `${this.name} makes a noise.`;
                        }
                    }
                    
                    class Dog extends Animal {
                        constructor(name, breed) {
                            super(name);
                            this.breed = breed;
                        }
                        
                        speak() {
                            return `${this.name} barks! Breed: ${this.breed}`;
                        }
                    }
                    
                    const dog = new Dog('旺财', '柴犬');
                    dog.speak();
                ").ToString();
                
                Console.WriteLine($"结果: {inheritanceResult}\n");

                // 测试4: 静态方法
                Console.WriteLine("测试4: 静态方法");
                string staticMethodResult = engine.Evaluate(@"
                    class MathHelper {
                        static add(a, b) {
                            return a + b;
                        }
                        
                        static multiply(a, b) {
                            return a * b;
                        }
                    }
                    
                    `加法结果: ${MathHelper.add(5, 3)}, 乘法结果: ${MathHelper.multiply(4, 2)}`;
                ").ToString();
                
                Console.WriteLine($"结果: {staticMethodResult}\n");

                // 测试5: Getters和Setters
                Console.WriteLine("测试5: Getters和Setters");
                string getterSetterResult = engine.Evaluate(@"
                    class Rectangle {
                        constructor(width, height) {
                            this._width = width;
                            this._height = height;
                        }
                        
                        get area() {
                            return this._width * this._height;
                        }
                        
                        set width(value) {
                            this._width = value;
                        }
                        
                        set height(value) {
                            this._height = value;
                        }
                    }
                    
                    const rect = new Rectangle(5, 10);
                    const area1 = rect.area;
                    
                    rect.width = 10;
                    rect.height = 20;
                    
                    const area2 = rect.area;
                    
                    `原始面积: ${area1}, 修改后面积: ${area2}`;
                ").ToString();
                
                Console.WriteLine($"结果: {getterSetterResult}\n");

                Console.WriteLine("所有测试都成功! Jint引擎支持ES6类和构造函数。");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"错误: {ex.Message}");
                Console.WriteLine("Jint可能不完全支持ES6类和构造函数。");
            }
        }
    }
} 