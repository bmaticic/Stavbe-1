import { IEchartData } from '../_models/echart-data';
import { FetchDataService } from '../_services/fetch-data.service';
import { MojElektroService } from '../_services/moj-elektro.service';
import { IRange } from '../_models/i-range';
import { Observable } from 'rxjs';

export function fetchEchartData(
  fetchDataService: FetchDataService,
  mojElektroService: MojElektroService,
  selectedRange: IRange,
  sifraAgregacija: string,
  sifraEnergijaMoc: string,
  letoOD: number,
  letoDO: number,
  mesecOD: number,
  mesecDO: number,
  setLoading: (loading: boolean) => void,
  setError: (error: string | null) => void,
  setChartData: (data: IEchartData | null) => void,
  enotniIdentifikator?: string,
  passedIdJavnegaObjekta?: number
): void {
  setError(null);
  setChartData(null);
  const rangeValues = selectedRange.value;
  if (!rangeValues || rangeValues.length !== 2) {
    setError('Invalid date range');
    return;
  }

  const selectedEnotniIdentifikator = enotniIdentifikator ?? mojElektroService.mojElektroSignal()?.enotniIdentifikator;
  const idJavnegaObjekta = passedIdJavnegaObjekta ?? mojElektroService.mojElektroSignal()?.idJavnegaObjekta;

  if (!selectedEnotniIdentifikator) {
    setError('No merilno mesto selected');
    return;
  }
  if (!idJavnegaObjekta) {
    setError('No javni objekt associated with selected merilno mesto');
    return;
  }
  setLoading(true);

  fetchDataService.fetchEchartData(
    idJavnegaObjekta,
    selectedEnotniIdentifikator,
    sifraAgregacija,
    sifraEnergijaMoc,
    letoOD,
    letoDO,
    mesecOD,
    mesecDO
  ).subscribe({
    next: (data: IEchartData) => {
      setChartData(data);
      setLoading(false);
    },
    error: (err: any) => {
      console.error(err);
      setError(err?.message ?? 'Error loading data');
      setLoading(false);
    }
  });
}