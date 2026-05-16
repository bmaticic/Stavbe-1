import { ChangeDetectionStrategy, Component, Input, SimpleChanges } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { EChartsCoreOption } from 'echarts/core';
// import echarts core
import * as echarts from 'echarts/core';
// import necessary echarts components
import { BarChart } from 'echarts/charts';
import { LineChart } from 'echarts/charts';
import { GridComponent, LegendComponent, TitleComponent, ToolboxComponent, TooltipComponent, DataZoomComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { CommonModule } from '@angular/common';
import { Egraf } from '../../_models/egraf';
import { IEchartData } from '../../_models/echart-data';
echarts.use([BarChart, LineChart, GridComponent, TitleComponent, ToolboxComponent, TooltipComponent, LegendComponent, DataZoomComponent, CanvasRenderer]);

@Component({
  selector: 'app-echart-graf',
  changeDetection: ChangeDetectionStrategy.OnPush,
  imports: [CommonModule, NgxEchartsDirective],
  templateUrl: './echart-graf.component.html',
  styleUrl: './echart-graf.component.css',
  providers: [
    provideEchartsCore({ echarts }),
  ]
})
export class EchartGrafComponent {
  // Input to receive selected merilno mesto data prirejeno za Echarts
  // @Input() eChartsEnergijaAPlus: IEchartData | null = null;    
  @Input() selectedMerilnoMestoGraf: Egraf | null = null;      // isti za energijaAPlus in za prejetaDelovnaMoč

  chartOption!: EChartsCoreOption;

  ngOnChanges(changes: SimpleChanges) {
    const change = changes['selectedMerilnoMestoGraf'];
    if (!change.isFirstChange() && this.selectedMerilnoMestoGraf) {

      // Define color palette for indices 0-5
      const colorPalette = [
        '#1976d2', // blue
        '#388e3c', // green
        '#fbc02d', // yellow
        '#d32f2f', // red
        '#7b1fa2', // purple
        '#00838f'  // teal
      ];

      // Map each value to its color using bloki[]
      const dataWithColor = (this.selectedMerilnoMestoGraf.vrednosti || []).map((val, i) => ({
        value: val,
        itemStyle: {
          color: colorPalette[
            (this.selectedMerilnoMestoGraf?.bloki && typeof this.selectedMerilnoMestoGraf.bloki[i] === 'number')
              ? this.selectedMerilnoMestoGraf.bloki[i]
              : 0
          ]
        }
      }));

      this.chartOption = {
        tooltip: {
          trigger: 'axis',
          position: function (pt: any[]) {
            return [pt[0], '10%'];
          }
        },
        title: {
          left: 'center',
          text: '15 minutne meritve porabe električne energije',
        },
        toolbox: {
          show: true,
          feature: {
            magicType: {
              type: ['bar']
            },
            dataZoom: {
              yAxisIndex: 'none',
            },
          },
        },
        dataZoom: [
          {
            type: 'slider',
            xAxisIndex: 0,
            start: 0,
            end: 100,
            bottom: 10,
            handleSize: '100%',
            showDetail: false
          }
        ],
        xAxis: {
          type: 'category',
          data: this.selectedMerilnoMestoGraf.axisXLabele,
        },
        yAxis: {
          axisLabel: {
            formatter: function (value: any) {
              return value.toLocaleString('sl-SI') + ' kWh';
            },
          },
        },
        series: [
          {
            data: dataWithColor,
            type: 'bar',
            smooth: true
          },
        ],
      };
    }
  }

}
