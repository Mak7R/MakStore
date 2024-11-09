import {useQuery} from "@tanstack/react-query";
import useAuthAxios from "@/hooks/use-auth-axios";
import {getUrlValuableParts} from "@/utils/url/url-functions";


function useAppQuery<T>(endpoint: string)
{
  const axios = useAuthAxios();
  
  return useQuery({
    queryKey: getUrlValuableParts(endpoint),
    queryFn: () => axios.get<T>(endpoint).then(res => res.data),
  });
}

export default useAppQuery;