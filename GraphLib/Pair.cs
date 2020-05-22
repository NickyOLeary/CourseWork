using System;

namespace GraphLib
{
    /// <summary>
    /// Класс, представляющий пару из 2-х 
    /// элементов - первый типа <typeparamref name="T"/> и 
    /// второй типа <typeparamref name="U"/>. 
    /// </summary>
    /// <typeparam name="T">
    /// Тип для первого элемента в паре.
    /// </typeparam>
    /// <typeparam name="U">
    /// Тип дл второго элемента в паре.
    /// </typeparam>
    public class Pair<T, U> : IComparable<Pair<T, U>>
        where T : IComparable<T>, new()
        where U : IComparable<U>, new()
    {
        #region Элементы пары
        /// <summary>
        /// Поле для свойства <see cref="First"/>.
        /// </summary>
        T first;
        /// <summary>
        /// Первый элемент в паре.
        /// </summary>
        /// <remarks>
        /// Поле данного свойства - <see cref="first"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Если тип <see cref="T"/> является ссылочным и 
        /// поданное значение является null.
        /// </exception>
        public T First
        {
            get { return first; }
            set
            {
                if (typeof(T).IsByRef && value == null)
                    throw new ArgumentNullException(
                        typeof(T) + " is type by reference, " +
                        "so first element in pair must not " +
                        "be null. ");
                first = value;
            }
        }

        /// <summary>
        /// Поле для свойства <see cref="Second"/>.
        /// </summary>
        U second;
        /// <summary>
        /// Второй элемент в паре.
        /// </summary>
        /// <remarks>
        /// Поле данного свойства - <see cref="second"/>.
        /// </remarks>
        /// <exception cref="ArgumentNullException">
        /// Если тип <see cref="U"/> является ссылочным и 
        /// поданное значение является null.
        /// </exception>
        public U Second
        {
            get { return second; }
            set
            {
                if (typeof(U).IsByRef && value == null)
                    throw new ArgumentNullException(
                        typeof(U) + " is type by reference, " +
                        "so second element in pair must not " +
                        "be null. ");
                second = value;
            }
        }
        #endregion

        #region Конструкторы
        /// <summary>
        /// Присваивает элементам пары значения по умолчанию 
        /// (через конструкторы по умолчанию данных типов).
        /// </summary>
        public Pair()
        {
            First = new T();
            Second = new U();
        }
        /// <summary>
        /// Присвивает элементам пары значения, переданные 
        /// через параметры.
        /// </summary>
        /// <param name="_first">
        /// Значение для первого элемента пары.
        /// </param>
        /// <param name="_second">
        /// Значение для второго элемента пары. 
        /// </param>
        public Pair(T _first, U _second)
        {
            First = _first;
            Second = _second;
        }
        #endregion

        #region Переопределённые базовые методы
        /// <summary>
        /// Определяет, равен ли заданный объект текущему объекту.
        /// </summary>
        /// <param name="obj">
        /// Объект, который требуется сравнить с текущим объектом.
        /// </param>
        /// <returns>
        /// Значение true, если указанный объект равен текущему объекту;
        /// в противном случае — значение false.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (obj is null) return false;
            return Equals(obj as Pair<T, U>);
        }
        /// <summary>
        /// Определяет, равен ли заданный объект типа <see cref="Pair{T, U}"/> 
        /// текущему.
        /// </summary>
        /// <param name="other">
        /// Объект типа <see cref="Pair{T, U}"/>, который требуется 
        /// сравнить с текущим объектом.
        /// </param>
        /// <returns>
        /// Значение true, если указанный объект типа <see cref="Pair{T, U}"/> 
        /// равен текущему объекту; в противном случае — значение false.
        /// </returns>
        public bool Equals(Pair<T, U> other)
        {
            if (other is null) return false;
            return (this == other);
        }

        /// <summary>
        /// Служит хэш-функцией для объектов типа <see cref="Pair{T, U}"/>.
        /// </summary>
        /// <returns>
        /// Хэш-код текущего объекта типа <see cref="Pair{T, U}"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return Tuple.Create(First, Second).GetHashCode();
        }

        /// <summary>
        /// Возвращает строку, представляющую текущий объект типа 
        /// <see cref="Pair{T, U}"/>.
        /// </summary>
        /// <returns>
        /// Строку, представляющую текущий объект типа <see cref="Pair{T, U}"/>.
        /// </returns>
        public override string ToString()
        {
            return $"Type of first element: {typeof(T)}; " +
                $"first element: {First}; type of second " +
                $"element: {typeof(U)}; second element: " +
                $"{Second}. ";
        }
        #endregion

        #region Реализация интерфейса IComparable<Pair<T, U>>
        /// <summary>
        /// Сравнивает заданный объект типа <see cref="Pair{T, U}"/> 
        /// с текущим и возвращает целое число, отражающее положение 
        /// текущего объекта относительно других в сортировке. 
        /// </summary>
        /// <param name="other">
        /// Заданный объект для сравнения с текущим.
        /// </param>
        /// <returns>
        /// 1 - если текущий объект стоит выше в сортировке;
        /// (-1) - если текущий объект стоит ниже в сортировке;
        /// 0 - если положение объектов в сортировке равнозначно.
        /// </returns>
        public int CompareTo(Pair<T, U> other)
        {
            int result;
            if ((result = First.CompareTo(other.First)) == 0)
                return Second.CompareTo(other.Second);
            return result;
        }
        #endregion

        #region Операторы сравнения
        public static bool operator >(Pair<T, U> p1, Pair<T, U> p2)
        {
            return (p1.CompareTo(p2) > 0);
        }
        public static bool operator <(Pair<T, U> p1, Pair<T, U> p2)
        {
            return (p1.CompareTo(p2) < 0);
        }
        public static bool operator >=(Pair<T, U> p1, Pair<T, U> p2)
        {
            return (p1.CompareTo(p2) >= 0);
        }
        public static bool operator <=(Pair<T, U> p1, Pair<T, U> p2)
        {
            return (p1.CompareTo(p2) <= 0);
        }
        public static bool operator ==(Pair<T, U> p1, Pair<T, U> p2)
        {
            return (p1.CompareTo(p2) == 0);
        }
        public static bool operator !=(Pair<T, U> p1, Pair<T, U> p2)
        {
            return (p1.CompareTo(p2) != 0);
        }
        #endregion
    }
}
