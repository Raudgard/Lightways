using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace Fields
{
    public static class ListExtensionGetRandomNext
    {
        /// <summary>
        /// ���������� ��������� �������� �� ������.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="withDeleting">� ��������� ����� ��������.</param>
        /// <returns></returns>
        public static T GetRandomNext<T>(this List<T> list, bool withDeleting = true)
        {
            T member = list[new System.Random().Next(0, list.Count)];
            if (withDeleting) list.Remove(member);
            return member;
        }

        /// <summary>
        /// ���������� ��������� �������� �� �������.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="array"></param>
        /// <returns></returns>
        public static T GetRandomNext<T>(this T[] array)
        {
            T member = array[new System.Random().Next(0, array.Length)];
            return member;
        }


        /// <summary>
        /// ���������� ��������� �������� �� ������.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="index">������ ���������� ��������.</param>
        /// <param name="withDeleting">� ��������� ����� ��������.</param>
        /// <returns></returns>
        public static T GetRandomNext<T>(this List<T> list, out int index, bool withDeleting = true)
        {
            index = new System.Random().Next(0, list.Count);
            T member = list[index];
            if (withDeleting) list.Remove(member);
            return member;
        }




        /// <summary>
        /// ���������� ��������� ���� �������� �� �������.
        /// </summary>
        /// <typeparam name="Tkey">����.</typeparam>
        /// <typeparam name="TValue">��������.</typeparam>
        /// <param name="dictionary">�������, �� ������� ����� �������� ��������� ���� ����-��������.</param>
        /// <param name="withDeleting">� ��������� ���� ����.</param>
        /// <returns></returns>
        public static KeyValuePair<Tkey, TValue> GetRandomNext<Tkey, TValue>(this Dictionary<Tkey, TValue> dictionary, bool withDeleting = true)
        {
            KeyValuePair<Tkey, TValue> member = dictionary.ElementAt(new System.Random().Next(0, dictionary.Count));
            if (withDeleting) dictionary.Remove(member.Key);
            return member;
        }



    }

    public static class ListExtensionGetRandomPercent
    {
        /// <summary>
        /// ���������� ����� ������, ��������� �� ��������� ������ ����������� ������ � ���������� ���������� ��������.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">��������� ������.</param>
        /// <param name="percent">������� ������ ������, ������� ���������� ��������.</param>
        /// <returns></returns>
        public static List<T> GetRandomPercent<T>(this List<T> list, int percent)
        {
            var res = new List<T>();
            var copyList = new List<T>(list);
            //Debug.Log($"list.Count: {list.Count}");

            percent = Mathf.Clamp(percent, 0, 100);
            int numberOfMembers = list.Count * percent / 100;
            //Debug.Log($"numberOfMembers: {numberOfMembers}");

            while(res.Count < numberOfMembers)
            {
                res.Add(copyList.GetRandomNext());
            }

            //Debug.Log($"list.Count: {list.Count}, copyList.Count: {copyList.Count}");
            return res;
        }
    }


    public static class ListExtensionGetPercentNext
    {
        /// <summary>
        /// ���������� ���� �� ������ ������, ������������ � ������ �� ��������� ��������, ������, ��� 0% - ������ ���� ������, 100% - ��������� ���� ������.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">��������� ������.</param>
        /// <param name="percent">�������, �� ������� ��������� ����������� ���� ������.</param>
        /// <returns></returns>
        public static T GetPercentNext<T>(this List<T> list, int percent, out int index)
        {
            var newpercent = Mathf.Clamp(percent, 0, 100) - 1;
            float i = newpercent * list.Count * 0.01f;
            index = (int)i;
            return list[index];
        }
    }


    public static class ListExtensionEverythingInARow
    {
        /// <summary>
        /// ���������� ������ �� ����� ������� � ��������� ������� ����� ��������� �����������.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list">��������� ������.</param>
        /// <param name="separator">�����������.</param>
        /// <returns></returns>
        public static string ToStringEverythingInARow<T>(this List<T> list, char separator = ',')
        {
            string res = string.Empty;
            //Debug.Log($"list.Count: {list.Count}");

            foreach (var u in list)
            {
                res += $"{u}{separator} ";
            }

            return res;
        }
    }






    public static class DirectionExtension
    {
        /// <summary>
        /// ���������� �����������, ��������������� ���������.
        /// </summary>
        /// <param name="direction">�����������, ������� ����� �������������.</param>
        /// <returns>��������������� �����������.</returns>
        public static Direction Opposite(this Direction direction)
        {
            var opposite = (int)direction + 180;
            return (Direction)(opposite > 359 ? opposite - 360 : opposite);
        }

        /// <summary>
        /// �������� �� ����������� ������������.
        /// </summary>
        /// <param name="direction">����������� �����������.</param>
        public static bool IsDiagonal(this Direction direction)
        {
            return (int)direction % 10 != 0;
        }
    }




}


public static class Vector3Extension
{
    /// <summary>
    /// ���������� ������ ��� �� �����, �� ���������� � ��������� �����������. ��������� ���������� Z ����������.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector3 TurnOnDirection(this Vector3 vector, Direction direction)
    {
        Vector2 vector2 = vector;
        Vector2 turnedVector2 = vector2.TurnOnDirection(direction);
        return new Vector3(turnedVector2.x, turnedVector2.y, vector.z);
    }

    
}

public static class Vector2Extension
{
    /// <summary>
    /// ���������� ������ ��� �� �����, �� ���������� � ��������� �����������.
    /// </summary>
    /// <param name="vector"></param>
    /// <param name="direction"></param>
    /// <returns></returns>
    public static Vector2 TurnOnDirection(this Vector2 vector, Direction direction)
    {
        float magnitude = vector.magnitude;
        Vector2 vectorOnRight = new Vector2(magnitude, 0);

        return direction switch
        {
            Direction.Up => new Vector2(0, magnitude),
            Direction.UpLeft => Quaternion.Euler(0, 0, 135) * vectorOnRight,
            Direction.Left => new Vector2(-magnitude, 0),
            Direction.DownLeft => Quaternion.Euler(0, 0, -135) * vectorOnRight,
            Direction.Down => new Vector2(0, -magnitude),
            Direction.DownRight => Quaternion.Euler(0, 0, -45) * vectorOnRight,
            Direction.Right => new Vector2(magnitude, 0),
            Direction.UpRight => Quaternion.Euler(0, 0, 45) * vectorOnRight,
            _ => throw new System.Exception()
        };
    }


}







