import { ChangeDetectionStrategy, Component, Input, SimpleChanges } from '@angular/core';
import { NgxEchartsDirective, provideEchartsCore } from 'ngx-echarts';
import { EChartsCoreOption } from 'echarts/core';
// import echarts core
import * as echarts from 'echarts/core';
// import necessary echarts components
import { BarChart } from 'echarts/charts';
import { LineChart } from 'echarts/charts';
import { GridComponent } from 'echarts/components';
import { CanvasRenderer } from 'echarts/renderers';
import { CommonModule } from '@angular/common';
import { Egraf } from '../../_models/egraf';
import { IEchartData } from '../../_models/echart-data';
echarts.use([BarChart, LineChart, GridComponent, CanvasRenderer]);

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


  ngOnChanges(changes: SimpleChanges) {
    const change = changes['selectedMerilnoMestoGraf'];
    if (!change.isFirstChange() && this.selectedMerilnoMestoGraf) {
      this.chartOption = {
        xAxis: {
          type: 'category',
          data: this.selectedMerilnoMestoGraf.axisXLabele,
        },
        yAxis: {
          type: 'value',
        },
        series: [
          {
            data: this.selectedMerilnoMestoGraf.vrednosti,
            type: 'bar',
            smooth: true,
          },
        ],
      };
    }
  }


  chartOption: EChartsCoreOption = {
    xAxis: {
      type: 'time',
      data: [],
    },
    yAxis: {
      type: 'value',
    },
    series: [
      {
        data: [],
        type: 'line',
      },
    ],
  };

  
}
