using EPI.CSharp.Model.Enums;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Asteroids.Indicators
{
    public class IndicatorGraph
    {
        private List<Color> _colors;

        private List<double> _values;

        public IndicatorGraph()
        {
            _values = new List<double>();
            _colors = new List<Color>();
        }
        /// <summary>
        /// 图形名称
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// 图形标签
        /// </summary>
        public string Tag { get; set; }
        /// <summary>
        /// 图形样式
        /// </summary>
        public EnumLineStyle LineStyle { get; set; }
        /// <summary>
        /// 图形颜色
        /// </summary>
        public List<Color> Colors
        {
            get { return _colors; }
        }
        /// <summary>
        /// 图形值
        /// </summary>
        public List<double> Values
        {
            get { return _values; }
        }

        /// <summary>
        /// 添加值
        /// </summary>
        /// <param name="value"></param>
        /// <param name="color"></param>
        public void AddValue(double value, Color color)
        {
            _values.Add(value);
            _colors.Add(color);
        }

        public void RemoveLast()
        {
            _values.RemoveAt(_values.Count - 1);
            _colors.RemoveAt(_colors.Count - 1);
        }
        /// <summary>
        /// 清空记录
        /// </summary>
        public void Clear()
        {
            _values.Clear();
            _colors.Clear();
        }
    }
}
