

export default interface IResponse<T> {
  status: number;
  statusText: string;
  data: T;
}