import { ProductOutput } from "./ProductOutput";

export interface ProductOutputPagedResultDto {
    totalCount?: number;
    items?: Array<ProductOutput> | null;
}