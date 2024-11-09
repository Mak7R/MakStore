export function getUrlValuableParts(url: string) : string[] {
  const cleanedUrl = url.replace(/^(https?:\/\/[^/]+\/api)/, '');
  
  const trimmedUrl = cleanedUrl.replace(/^\/+|\/+$/g, '');
  
  return trimmedUrl.split('/');
}