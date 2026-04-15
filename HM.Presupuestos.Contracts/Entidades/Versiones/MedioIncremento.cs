using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace HM.Presupuestos.Contratos.Entidades
{
    public class MedioIncremento : INotifyPropertyChanged
    {
        public int CodigoMedio { get; set; }
        public string DescripcionMedio { get; set; } = string.Empty;
        
        private decimal _netoVentaOrigen;
        private decimal _importe;
        private decimal _incrementoPorcentaje;
    
        /// <summary>
        /// Neto de venta origen (valor base para cálculos)
        /// </summary>
        public decimal NetoVentaOrigen
        {
            get => _netoVentaOrigen;
            set
            {
                if (_netoVentaOrigen != value)
                {
                    _netoVentaOrigen = Math.Round(value, 2);
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(NetoVentaTotal));
                }
            }
        }

        /// <summary>
        /// Porcentaje de incremento (cuando cambia, recalcula el Importe)
        /// Mantiene precisión de máximo 6 decimales
        /// </summary>
        public decimal IncrementoPorcentaje
        {
            get => _incrementoPorcentaje;
            set
            {
                // Limitar a 6 decimales máximo
                var valorLimitado = Math.Round(value, 6);

                if (_incrementoPorcentaje != valorLimitado)
                {
                    _incrementoPorcentaje = valorLimitado;

                    // Recalcular Importe basado en el nuevo porcentaje
                    _importe = Math.Round(_netoVentaOrigen * _incrementoPorcentaje / 100, 2);

                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Importe));
                    OnPropertyChanged(nameof(NetoVentaTotal));
                }
            }
        }

        /// <summary>
        /// Importe de variación (cuando cambia, recalcula el Porcentaje)
        /// </summary>
        public decimal Importe
        {
            get => _importe;
            set
            {
                var roundedValue = Math.Round(value, 2);
                if (_importe != roundedValue)
                {
                    _importe = roundedValue;
                    
                    // Recalcular Porcentaje basado en el nuevo importe (limitado a 6 decimales)
                    if (_netoVentaOrigen != 0)
                    {
                        _incrementoPorcentaje = Math.Round((_importe / _netoVentaOrigen) * 100, 6);
                    }
                    else
                    {
                        _incrementoPorcentaje = 0;
                    }
                    
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IncrementoPorcentaje));
                    OnPropertyChanged(nameof(NetoVentaTotal));
                }
            }
        }

        /// <summary>
        /// Neto de venta total (calculado automáticamente)
        /// </summary>
        public decimal NetoVentaTotal
        {
            get => Math.Round(_netoVentaOrigen + _importe, 2);
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion
    }
}
